using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Dictionary<string, int> tropicalVehicle = new Dictionary<string, int>();
    Dictionary<string, int> dryVehicle = new Dictionary<string, int>();
    Dictionary<string, int> mildVehicle = new Dictionary<string, int>();
    Dictionary<string, int> continentalVehicle = new Dictionary<string, int>();
    Dictionary<string, int> polarVehicle = new Dictionary<string, int>();
    Dictionary<string, Dictionary<string, int>> climateVehicles = new Dictionary<string, Dictionary<string, int>>();

    const int IDEAL = 1;
    const int ADEQUATE = 2;
    const int DIFFICULT = 3;
    public const int IMPOSSIBLE = 0;

    public void InitVehicles()
    {
        InitClimate();
    }

    public Dictionary<string, int> GetClimateVehicle(string vehicleType)
    {
        return climateVehicles[vehicleType];
    }

    void InitClimate()
    {
        InitTropical();
        InitDry();
        InitMild();
        InitContinental();
        InitPolar();

        climateVehicles.Add("Tropical", tropicalVehicle);
        climateVehicles.Add("Dry", dryVehicle);
        climateVehicles.Add("Mild", mildVehicle);
        climateVehicles.Add("Continental", continentalVehicle);
        climateVehicles.Add("Polar", polarVehicle);
    }

    void InitTropical()
    {
        tropicalVehicle.Add("Tropical", IDEAL);
        tropicalVehicle.Add("Dry", IMPOSSIBLE);
        tropicalVehicle.Add("Mild", ADEQUATE);
        tropicalVehicle.Add("Continental", DIFFICULT);
        tropicalVehicle.Add("Polar", IMPOSSIBLE);
    }

    void InitDry()
    {
        dryVehicle.Add("Tropical", IMPOSSIBLE);
        dryVehicle.Add("Dry", IDEAL);
        dryVehicle.Add("Mild", ADEQUATE);
        dryVehicle.Add("Continental", DIFFICULT);
        dryVehicle.Add("Polar", IMPOSSIBLE);
    }

    void InitMild()
    {
        mildVehicle.Add("Tropical", DIFFICULT);
        mildVehicle.Add("Dry", DIFFICULT);
        mildVehicle.Add("Mild", IDEAL);
        mildVehicle.Add("Continental", ADEQUATE);
        mildVehicle.Add("Polar", IMPOSSIBLE);
    }

    void InitContinental()
    {
        continentalVehicle.Add("Tropical", IMPOSSIBLE);
        continentalVehicle.Add("Dry", IMPOSSIBLE);
        continentalVehicle.Add("Mild", ADEQUATE);
        continentalVehicle.Add("Continental", IDEAL);
        continentalVehicle.Add("Polar", DIFFICULT);
    }

    void InitPolar()
    {
        polarVehicle.Add("Tropical", IMPOSSIBLE);
        polarVehicle.Add("Dry", IMPOSSIBLE);
        polarVehicle.Add("Mild", DIFFICULT);
        polarVehicle.Add("Continental", ADEQUATE);
        polarVehicle.Add("Polar", IDEAL);
    }
}