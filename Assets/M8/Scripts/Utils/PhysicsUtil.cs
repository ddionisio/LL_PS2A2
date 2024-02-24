using UnityEngine;
//using System.Collections.Generic;

namespace M8 {
    public struct PhysicsUtil {
        const float defaultSphereCosCheck = 0.86602540378443864676372317075294f;
        const float defaultBoxCosCheck = 0.70710678118654752440084436210485f;

        public static int[] GetLayerIndices(int layerMask) {
            int[] layers = new int[32];
            int numLayers = 0;

            for(int i = 0; layerMask != 0; layerMask >>= 1, i++) {
                if((layerMask & 1) != 0) {
                    layers[numLayers] = i;
                    numLayers++;
                }
            }
                        
            if(numLayers > 0) {
                System.Array.Resize(ref layers, numLayers);
                return layers;
            }

            return null;
        }
    }
}