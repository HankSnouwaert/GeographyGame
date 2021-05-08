using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Contains general information and methods relating to mappable objects, 
    /// along with other interfaces related to more specific types of mappable objects
    /// </summary>
    public interface IMappablesManager
    {
        /// <summary> 
        /// Contains list of all mappable objects on world map globe
        /// </summary>
        Dictionary<string, MappableObject> MappedObjects { get; set; }

        /// <summary> 
        /// Contains information and functionality relating to landmarks
        /// </summary>
        ILandmarkManager LandmarkManager { get; }

        /// <summary> 
        /// Instantiates the mappable objects specified by the mount points of a given country
        /// </summary>
        /// <param name="country"> The country whose mappable objects are being instantiated </param>
        void IntantiateMappables(Country country);
    }
}