// World Political Map - Globe Edition for Unity - Main Script
// Created by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WPM

using UnityEngine;
using System;

namespace WPM {


    public partial class WorldMapGlobe : MonoBehaviour {

        float currentYaw, currentPitch;
        bool pitchIsMaxed;
        Transform pitchTransform, yawTransform;
        Transform _pivotTransform;

        public Transform pivotTransform {
            get {
                if (_pivotTransform == null) {
                    CheckCameraPivot();
                }
                return _pivotTransform;
            }
        }

        void OrbitStart() {
            currentPitch = pitch;
            currentYaw = yaw;
        }

        void CheckCameraPivot() {
            Camera cam = mainCamera;
            if (cam == null) return;
            Transform camParent = cam.transform.parent;
            if (_enableOrbit) {
                if (camParent == null || !camParent.name.Equals("Yaw")) {
                    // add pivot
                    GameObject pivot = new GameObject("CameraPivot");
                    _pivotTransform = pivot.transform;
                    if (camParent != null) {
                        pivotTransform.SetParent(camParent, false);
                    }
                    pivot.transform.position = cam.transform.position;
                    pivot.transform.rotation = cam.transform.rotation;

                    // add pitch
                    GameObject pitchPivot = new GameObject("Pitch");
                    pitchTransform = pitchPivot.transform;
                    pitchTransform.SetParent(pivotTransform, false);

                    // add yaw
                    GameObject yawPivot = new GameObject("Yaw");
                    yawTransform = yawPivot.transform;
                    yawTransform.SetParent(pitchTransform, false);

                    // reparent cam
                    cam.transform.SetParent(yawTransform, false);
                } else {
                    yawTransform = cam.transform.parent;
                    pitchTransform = yawTransform.parent;
                    _pivotTransform = pitchTransform.parent;
                }
            } else {
                if (camParent != null && camParent.name.Equals("Yaw")) {
                    Transform grandParent = camParent.transform.parent;
                    if (grandParent != null && grandParent.name.Equals("Pitch")) {
                        Transform root = grandParent.parent;
                        if (root != null) {
                            cam.transform.SetParent(root.parent, true);
                        }
                        DestroyImmediate(root.gameObject);
                    }
                }
                pitchTransform = yawTransform = null;
                _pivotTransform = cam.transform;
            }
        }

        void RotateCameraAroundAxis(Camera cam, float angle) {
            angle *= Time.deltaTime * 60f;

            Vector3 axis = (transform.position - pivotTransform.position).normalized;
            if (_navigationMode == NAVIGATION_MODE.EARTH_ROTATES) {
                transform.Rotate(axis, angle, Space.World);
            } else {
                pivotTransform.Rotate(axis, angle, Space.World);
            }
        }

        void PerformZoomInOut(Camera cam, float distance) {
            Vector3 vector = pivotTransform.forward * (Vector3.Distance(transform.position, pivotTransform.position) * distance * _mouseWheelSensitivity * Time.deltaTime * 60f);
            if (_zoomMode == ZOOM_MODE.CAMERA_MOVES) {
                pivotTransform.position = pivotTransform.position - vector;
            } else {
                transform.position = transform.position + vector;
            }
        }


        Vector3 GetZoomDir(Camera cam) {
            // Operate in double for better accuracy
            Vector3 v = pivotTransform.position - transform.position;
            double magnitude = System.Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
            Vector3 dir = new Vector3((float)(v.x / magnitude), (float)(v.y / magnitude), (float)(v.z / magnitude));
            return dir;
        }

        bool CheckCamMinMaxDistance(Camera cam) {
            Vector3 camPos = pivotTransform.position;
            float camDistSqr = FastVector.SqrDistanceByValue(camPos, transform.position);
            float minDist = _zoomMinDistance + radius + (cam.nearClipPlane + 0.01f);
            float minDistSqr = minDist * minDist;
            float maxDist = _zoomMaxDistance + radius + cam.nearClipPlane;
            float maxDistSqr = maxDist * maxDist;

            if (_zoomMode == ZOOM_MODE.CAMERA_MOVES) {
                if (camDistSqr < minDistSqr) {
                    pivotTransform.position = transform.position + GetZoomDir(cam) * minDist;
                    return false;
                } else if (camDistSqr > maxDistSqr) {
                    pivotTransform.position = transform.position + GetZoomDir(cam) * maxDist;
                    return false;
                }
            } else {
                if (camDistSqr < minDistSqr) {
                    transform.position = pivotTransform.position - GetZoomDir(cam) * minDist;
                    return false;
                } else if (camDistSqr > maxDistSqr) {
                    transform.position = pivotTransform.position - GetZoomDir(cam) * maxDist;
                    return false;
                }
            }
            return true;
        }

        void KeepCameraStraight() {
            if (currentPitch != 0) return;
            if (_keepStraight && !isFlyingToActive) {
                if (_navigationMode == NAVIGATION_MODE.EARTH_ROTATES) {
                    StraightenGlobe(SMOOTH_STRAIGHTEN_ON_POLES, true);
                } else {
                    pivotTransform.rotation = Quaternion.Lerp(pivotTransform.rotation, GetCameraStraightLookRotation(), 0.3f);
                }
            }
        }


        /// <summary>
        /// Manages two finger zoom and rotation
        /// </summary>
        /// <returns>Impulse for zooming</returns>
        float CheckTwoFingerZoomGestures(Camera cam) {
            // Check pinch gesture
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroDelta = touchZero.deltaPosition.normalized;
            Vector2 touchOneDelta = touchOne.deltaPosition.normalized;
            float pinch = Vector2.Dot(touchZeroDelta, touchOneDelta);
            if (pinch < -0.5f) {
                pinchZooming = true;

                // zooming or rotating?
                Vector2 center = (touchZero.position + touchOne.position) * 0.5f;
                Vector2 touchZeroToCenter = (center - touchZero.position).normalized;
                Vector2 touchOneToCenter = (center - touchOne.position).normalized;
                float dtZero = Mathf.Abs(Vector2.Dot(touchZeroToCenter, touchZeroDelta));
                float dtOne = Mathf.Abs(Vector2.Dot(touchOneToCenter, touchOneDelta));

                if (dtZero > 0.8f || dtOne > 0.8f) {
                    // Zoom in/out
                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                    deltaMagnitudeDiff = ApplyDragThreshold(deltaMagnitudeDiff, _mouseDragThreshold);
                    // Pass the delta to the wheel accel
                    if (deltaMagnitudeDiff != 0) {
                        wheelAccel = 0;
                        UpdateCursorLocation();
                        return deltaMagnitudeDiff * 0.001f;

                    }
                } else {
                    // Rotate with fingers
                    Vector2 v2 = touchZero.position - touchOne.position;
                    if (v2.sqrMagnitude > 10 * 10) {
                        float newAngle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                        if (oldPinchRotationAngle != 999) {
                            float rotAngle = Mathf.DeltaAngle(newAngle, oldPinchRotationAngle);
                            if (_navigationMode == NAVIGATION_MODE.EARTH_ROTATES) {
                                rotAngle *= -1f;
                            }
                            RotateCameraAroundAxis(cam, rotAngle);
                        }
                        oldPinchRotationAngle = newAngle;
                    }
                }
            }
            return 0;
        }

        void PerformDragDamping(Camera cam) {
            float t = 1f - (Time.time - dragDampingStart) / (dragDampingDuration + 0.001f);
            if (t < 0) {
                t = 0;
                dragDampingStart = 0;
            } else if (t > 1f) {
                t = 1f;
            }

            if (_navigationMode == NAVIGATION_MODE.EARTH_ROTATES) {
                transform.Rotate(cam.transform.up, dragDirection.x * t, Space.World);
                Vector3 axisY = Vector3.Cross(transform.position - cam.transform.position, cam.transform.up);
                transform.Rotate(axisY, dragDirection.y * t, Space.World);
            } else {
                if (_rotationAxisAllowed == ROTATION_AXIS_ALLOWED.X_AXIS_ONLY) {
                    pivotTransform.RotateAround(transform.position, cam.transform.up, -dragDirection.x * t);
                } else {
                    RotateAround(pivotTransform, transform.position, cam.transform.up, -dragDirection.x * t);
                    RotateAround(pivotTransform, transform.position, cam.transform.right, dragDirection.y * t);
                }
            }
        }

        void CheckAngleConstraint() {
            // Check constraint around position
            Vector3 oldCamPos = pivotTransform.position;
            Vector3 globePos = transform.position;
            transform.position = Misc.Vector3zero;
            pivotTransform.position -= globePos;
            Vector3 camPos = pivotTransform.position;
            Ray ray = new Ray(camPos, pivotTransform.forward);
            Vector3 hitPoint;
            if (GetGlobeIntersection(ray, out hitPoint)) {
                Vector3 contraintWPos = transform.TransformPoint(constraintPosition);
                Vector3 axis = Vector3.Cross(contraintWPos, hitPoint);
                float angleDiff = SignedAngleBetween(contraintWPos, hitPoint, axis);
                if (Mathf.Abs(angleDiff) > constraintPositionAngle + 0.0001f) {
                    if (angleDiff > 0) {
                        angleDiff = constraintPositionAngle - angleDiff;
                    } else {
                        angleDiff -= constraintPositionAngle;
                    }
                    if (_navigationMode == NAVIGATION_MODE.CAMERA_ROTATES) {
                        axis = Vector3.Cross(contraintWPos - camPos, hitPoint - camPos);
                        Vector3 prevUp = pivotTransform.up;
                        pivotTransform.Rotate(axis, angleDiff, Space.World);
                        pivotTransform.LookAt(camPos + pivotTransform.forward, prevUp); // keep straight
                    } else {
                        axis.z = 0;
                        transform.Rotate(axis, -angleDiff, Space.World);
                    }
                    dragDampingStart = 0;
                }
            }
            pivotTransform.position = oldCamPos;
            transform.position = globePos;
        }


        void CheckLatitudeConstraint() {
            Vector3 hitPos;
            Ray ray = new Ray(pivotTransform.position, pivotTransform.forward);
            if (!GetGlobeIntersection(ray, out hitPos)) return;
            hitPos = transform.InverseTransformPoint(hitPos);

            float sy = hitPos.y * 2f;
            float maxLat = Mathf.Sin(constraintLatitudeMaxAngle * Mathf.Deg2Rad);
            float minLat = Mathf.Sin(constraintLatitudeMinAngle * Mathf.Deg2Rad);

            float correction;
            if (sy > maxLat) {
                correction = maxLat - sy;
            } else if (sy < minLat) {
                correction = minLat - sy;
            } else {
                return;
            }

            float corr = Mathf.Asin(correction) * Mathf.Rad2Deg;
            if (_navigationMode == NAVIGATION_MODE.CAMERA_ROTATES) {
                Vector3 axis = Vector3.Cross(transform.up, _pivotTransform.forward);
                Vector3 prevUp = _pivotTransform.transform.up;
                RotateAround(pivotTransform, transform.position, axis, corr);
                pivotTransform.LookAt(transform.position, prevUp);
            } else {
                Vector3 axis = Vector3.Cross(transform.up, _pivotTransform.forward);
                transform.Rotate(axis, -corr, Space.World);
            }
            dragDampingStart = 0;
        }

        void CheckRotationKeys(Camera cam) {
            bool pressed = false;
            Vector3 dragKeyVert = Misc.Vector3zero;
            if (Input.GetKey(KeyCode.W)) {
                dragKeyVert = Misc.Vector3down;
                pressed = true;
            } else if (Input.GetKey(KeyCode.S)) {
                dragKeyVert = Misc.Vector3up;
                pressed = true;
            }
            Vector3 dragKeyHoriz = Misc.Vector3zero;
            if (Input.GetKey(KeyCode.A)) {
                dragKeyHoriz = Misc.Vector3right;
                pressed = true;
            } else if (Input.GetKey(KeyCode.D)) {
                dragKeyHoriz = Misc.Vector3left;
                pressed = true;
            }
            if (pressed) {
                dragDirection = dragKeyVert + dragKeyHoriz;
                float distFactor = Mathf.Min((Vector3.Distance(pivotTransform.position, transform.position) - radius) / radius, 1f);
                dragDirection *= distFactor * _mouseDragSensitivity;
                if (_dragConstantSpeed) {
                    dragDirection *= 18f;
                    dragDampingStart = Time.time + dragDampingDuration;
                } else {
                    dragDampingStart = Time.time;
                }
            }
        }

        void PerformDragOnScreenEdges() {
            bool onEdges = IsPointerOnScreenEdges();
            if (onEdges) {
                if (!mouseStartedDragging) {
                    mouseDragStart = Input.mousePosition;
                    mouseStartedDragging = true;
                }
                Vector2 delta = ((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * 0.5f).normalized;
                DragTowards(delta);
            }
        }


        public virtual void DragTowards(Vector2 dragDirection) {
            Camera cam = mainCamera;

            float distFactor = Mathf.Min((GetCameraDistance() - radius) / radius, 1f);
            if (_rotationAxisAllowed == ROTATION_AXIS_ALLOWED.X_AXIS_ONLY) {
                dragDirection.y = 0;
            } else {
                dragDirection.y = ApplyDragThreshold(dragDirection.y, _mouseDragThreshold);
            }
            if (_rotationAxisAllowed == ROTATION_AXIS_ALLOWED.Y_AXIS_ONLY) {
                dragDirection.x = 0;
            } else {
                dragDirection.x = ApplyDragThreshold(dragDirection.x, _mouseDragThreshold);
            }
            dragDirection *= distFactor * _dragOnScreenEdgesSpeed * Time.deltaTime * 60f;
            if (dragDirection.x != 0 || dragDirection.y != 0) {
                hasDragged = true;
                if (_navigationMode == NAVIGATION_MODE.EARTH_ROTATES) {
                    transform.Rotate(cam.transform.up, dragDirection.x, Space.World);
                    Vector3 axisY = Vector3.Cross(transform.position - cam.transform.position, cam.transform.up);
                    transform.Rotate(axisY, dragDirection.y, Space.World);
                } else {
                    if (_rotationAxisAllowed == ROTATION_AXIS_ALLOWED.X_AXIS_ONLY) {
                        pivotTransform.RotateAround(transform.position, cam.transform.up, -dragDirection.x);
                    } else {
                        RotateAround(pivotTransform, transform.position, cam.transform.up, -dragDirection.x);
                        RotateAround(pivotTransform, transform.position, cam.transform.right, dragDirection.y);
                    }
                }
            }
            StopAnyNavigation();
        }

        void PerformClickAndThrow(Camera cam) {
            dragAngle *= 0.9f;
            hasDragged = true;
            if (_navigationMode == NAVIGATION_MODE.EARTH_ROTATES) {
                Vector3 angles = Quaternion.AngleAxis(dragAngle, dragAxis).eulerAngles;
                transform.Rotate(angles);
            } else {
                RotateAround(pivotTransform, transform.position, dragAxis, -dragAngle);
            }
        }

        void PerformOrbit(Camera cam) {

            if (yawTransform == null) return;

            bool triggerRotateEnds = false;
            if (isOrbitRotateToActive) {
                float t = (Time.time - orbitRotateToStartTime) / orbitRotateToDuration;
                if (t >= 1f) {
                    t = 1f;
                    triggerRotateEnds = true;
                }
                t = Mathf.SmoothStep(0, 1, t);
                yaw = Mathf.Lerp(orbitRotateToYawStart, orbitRotateToYawEnd, t);
                pitch = Mathf.Lerp(orbitRotateToPitchStart, orbitRotateToPitchEnd, t);
            }

            Vector3 targetPosition = GetOrbitTargetPositionWS();
            float camDistanceToTargetPosition = Vector3.Distance(pivotTransform.position, targetPosition);
            if (_pitchAdjustByDistance) {
                float distanceLerp = (camDistanceToTargetPosition - _pitchMinDistanceTilt) / (_pitchMaxDistanceTilt - _pitchMinDistanceTilt);
                distanceLerp = Mathf.Clamp01(distanceLerp);
                float maxPitchByDistance = Mathf.Lerp(maxPitch, 0, distanceLerp);
                if (pitch > maxPitchByDistance || (_orbitTiltOnZoomIn && zoomDistance < 0 && pitchIsMaxed)) {
                    pitch = maxPitchByDistance;
                }
                pitchIsMaxed = pitch == maxPitchByDistance;
            }
            pitch = Mathf.Clamp(pitch, 0, _maxPitch);
            if (Application.isPlaying) {
                currentYaw = Mathf.Lerp(currentYaw, yaw, Time.deltaTime * _orbitSmoothSpeed);
                currentPitch = Mathf.Lerp(currentPitch, pitch, Time.deltaTime * _orbitSmoothSpeed);
                if (pitch == 0) {
                    const float EPSILON = 0.001f;
                    if (Mathf.Abs(currentPitch - pitch) < EPSILON) {
                        currentPitch = pitch;
                    }
                }
            } else {
                currentYaw = yaw;
                currentPitch = pitch;
            }

            pitchTransform.localRotation = Quaternion.Euler(-currentPitch, 0, 0);
            cam.transform.localRotation = Quaternion.identity;
            Vector3 pos = targetPosition - cam.transform.forward * camDistanceToTargetPosition;
            cam.transform.position = pos;
            Vector3 orbitAxis = (pivotTransform.position - targetPosition).normalized;
            RotateAround(cam.transform, targetPosition, orbitAxis, currentYaw);

            if (triggerRotateEnds) {
                StopRotateTo(true);
            }
        }


        Vector3 GetOrbitTargetPositionWS() {
            if (_orbitTarget != null) {
                return _orbitTarget.transform.position;
            }
            Vector3 orbitAxis = (pivotTransform.position - transform.position).normalized;
            float elevation = targetElevation * (radius / EARTH_RADIUS_KM);
            return transform.position + orbitAxis * (radius + elevation);
        }


        bool orbitRotateToComplete;
        float orbitRotateToYawStart, orbitRotateToYawEnd;
        float orbitRotateToPitchStart, orbitRotateToPitchEnd;
        float orbitRotateToStartTime, orbitRotateToDuration;

        public CallbackHandler OrbitRotateTo(float targetYaw, float targetPitch, float duration = 0.5f) {

            yaw %= 360;
            currentYaw %= 360;
            targetYaw %= 360;
            if (targetYaw - yaw > 180) {
                targetYaw -= 360;
            }
            if (targetYaw - yaw < -180) {
                targetYaw += 360;
            }

            orbitRotateToYawStart = yaw;
            orbitRotateToYawEnd = targetYaw;
            orbitRotateToPitchStart = pitch;
            orbitRotateToPitchEnd = targetPitch;

            orbitRotateToStartTime = Time.time;
            orbitRotateToDuration = duration;
            isOrbitRotateToActive = true;
            orbitRotateToComplete = false;

            if (OnOrbitRotateStart != null) {
                OnOrbitRotateStart();
            }

            return new CallbackHandlerForOrbitRotateTo(this);
        }


    }

}