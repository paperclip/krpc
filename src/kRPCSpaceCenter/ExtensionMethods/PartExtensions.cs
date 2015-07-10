using System.Collections.Generic;
using System.Linq;

namespace KRPCSpaceCenter.ExtensionMethods
{
    static class PartExtensions
    {
        /// <summary>
        /// Returns true if the part contains the given part module
        /// </summary>
        public static bool HasModule<T> (this Part part) where T : PartModule
        {
            return part.Modules.OfType<T> ().Any ();
        }

        /// <summary>
        /// Returns the first part module of the specified type, and null if none can be found
        /// </summary>
        public static T Module<T> (this Part part) where T : PartModule
        {
            return part.Modules.OfType<T> ().FirstOrDefault ();
        }

        /// <summary>
        /// Returns true if the part contributes to the physics simulation (e.g. it has mass)
        /// </summary>
        public static bool IsPhysicallySignificant (this Part part)
        {
            return (!part.HasModule<ModuleLandingGear> ()) &&
            (!part.HasModule<LaunchClamp> ()) &&
            (part.physicalSignificance != Part.PhysicalSignificance.NONE);
        }

        /// <summary>
        /// Returns the index of the stage in which the part will be decoupled,
        /// or 0 if it is never decoupled.
        /// </summary>
        public static int DecoupledAt (this Part part)
        {
            do {
                if (part.HasModule<ModuleDecouple> () || part.HasModule<ModuleAnchoredDecoupler> () || part.HasModule<LaunchClamp> ())
                    return part.inverseStage;
                part = part.parent;
            } while (part != null);
            return -1;
        }

        /// <summary>
        /// Returns the total mass of the part and any resources it contains, in kg.
        /// </summary>
        public static float TotalMass (this Part part)
        {
            return (part.mass + part.GetResourceMass ()) * 1000;
        }

        /// <summary>
        /// Returns the total mass of the part, excluding any resources it contains, in kg.
        /// </summary>
        public static float DryMass (this Part part)
        {
            return part.mass * 1000;
        }

        /// <summary>
        /// Return all parts from which fuel can flow to the given root part
        /// </summary>
        public static IEnumerable<Part> FuelFlowConnectedParts (this Part root)
        {
            var visited = new HashSet<Part> ();
            var parts = new Stack<Part> ();
            parts.Push (root);

            //FIXME: hack...
            if (root.parent != null)
                parts.Push (root.parent);

            while (parts.Count > 0) {
                var part = parts.Pop ();

                // See http://forum.kerbalspaceprogram.com/threads/64362
                // Rule #1
                if (visited.Contains (part))
                    continue;
                visited.Add (part);

                yield return part;

                // Rule #2 - parts connected by fuel lines
                foreach (var fuelLine in part.fuelLookupTargets)
                    parts.Push (fuelLine.parent);

                // Rule #4 - axially attached parents and children
                if (part.fuelCrossFeed) {
                    if (part.parent != null && part.attachMode == AttachModes.STACK)
                        parts.Push (part.parent);
                    foreach (var child in part.children) {
                        if (child.attachMode == AttachModes.STACK) {
                            parts.Push (child);
                        }
                    }
                }

                // Rule #7 - part is radially attached to parent and is crossfeed capable
                //if (part.fuelCrossFeed && part.parent != null && part.attachMode == AttachModes.SRF_ATTACH)
                //    parts.Push (part.parent);
            }
        }
    }
}
