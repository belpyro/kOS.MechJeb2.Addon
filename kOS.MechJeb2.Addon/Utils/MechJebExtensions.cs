using System;
using System.Linq.Expressions;
using System.Reflection;
using kOS.MechJeb2.Addon.Core;
using KSPBuildTools;

namespace kOS.MechJeb2.Addon.Utils
{
    public static class MechJebExtensions
    {
        private static Func<Vessel, PartModule> _cachedGetter;
        private static bool _initialized;
        
        public static PartModule GetMasterMJ(this Vessel vessel)
        {
            if (!_initialized)
                InitializeGetter();

            return vessel != null ? _cachedGetter(vessel) : null;
        }

        private static void InitializeGetter()
        {
            try
            {
                // Get MuMech.VesselExtensions type
                var extType = Constants.VesselExtensionName.GetTypeFromCache();
                if (extType == null)
                    throw new Exception("Cannot load MuMech.VesselExtensions");

                // Find method GetMasterMechJeb(Vessel)
                var method = extType.GetMethod("GetMasterMechJeb",
                    BindingFlags.Public | BindingFlags.Static);

                if (method == null)
                    throw new Exception("Cannot find GetMasterMechJeb");

                // Prepare lambda parameter
                var paramVessel = Expression.Parameter(typeof(Vessel), "v");

                // Build method call v → GetMasterMechJeb(v)
                var call = Expression.Call(method, paramVessel);

                // Convert to PartModule
                var convert = Expression.Convert(call, typeof(PartModule));

                // Compile into delegate
                _cachedGetter = Expression.Lambda<Func<Vessel, PartModule>>(
                    convert, paramVessel).Compile();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                _cachedGetter = _ => null; // fallback
            }

            _initialized = true;
        }
    }
}