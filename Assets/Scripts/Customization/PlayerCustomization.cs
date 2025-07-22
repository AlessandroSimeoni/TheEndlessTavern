using System.Collections.Generic;
using UnityEngine;

namespace Customization
{
    public class PlayerCustomization : MonoBehaviour
    {
        [SerializeField] private CustomizationSettings settings = null;
        [SerializeField] private GameObject rootBone = null;
        [Header("Armor Pieces Meshes")]
        [SerializeField] private SkinnedMeshRenderer sweaterMesh = null;
        [SerializeField] private SkinnedMeshRenderer handsMesh = null;
        [SerializeField] private SkinnedMeshRenderer pantsMesh = null;
        [SerializeField] private SkinnedMeshRenderer bootsMesh = null;

        private Dictionary<string, Transform> myBone = new Dictionary<string, Transform>();

        private void Start()
        {
            Transform[] availableBones = rootBone.GetComponentsInChildren<Transform>();
            foreach (Transform bone in availableBones)
                myBone[bone.name] = bone;

            SetMesh(PlayerPrefs.GetInt(CustomizationManager.PLAYER_ARMOR_PREF, 0));
            SetMaterial(PlayerPrefs.GetInt(CustomizationManager.PLAYER_MATERIAL_PREF, 0));
        }

        public void SetMesh(int index)
        {
            ArmorSet targetArmor = settings.armorSet[index];

            sweaterMesh.bones = GetTargetBones(targetArmor.armorRenderer.sweater);
            sweaterMesh.sharedMesh = targetArmor.sweater;

            handsMesh.bones = GetTargetBones(targetArmor.armorRenderer.hands);
            handsMesh.sharedMesh = targetArmor.hands;

            pantsMesh.bones = GetTargetBones(targetArmor.armorRenderer.pants);
            pantsMesh.sharedMesh = targetArmor.pants;

            bootsMesh.bones = GetTargetBones(targetArmor.armorRenderer.boots);
            bootsMesh.sharedMesh = targetArmor.boots;
        }

        /// <summary>
        /// Gets an array of bones from the available ones (in the scene) that matches the target skinned mesh renderer bones array.
        /// In this way, assigning the resulting array to the skinned mesh renderer of the player in the scene avoids the meshes to stretch badly
        /// </summary>
        /// <param name="targetSkinnedRenderer">the target skinned mesh renderer</param>
        /// <returns>the correct array of bones to pass at the skinnedMeshRenderer</returns>
        private Transform[] GetTargetBones(SkinnedMeshRenderer targetSkinnedRenderer)
        {
            Transform[] targetBones = new Transform[targetSkinnedRenderer.bones.Length];
            for (int i = 0; i < targetBones.Length; i++)
                targetBones[i] = myBone[targetSkinnedRenderer.bones[i].name];

            return targetBones;
        }

        public void SetMaterial(int index)
        {
            int armorIndex = PlayerPrefs.GetInt(CustomizationManager.PLAYER_ARMOR_PREF, 0);
            Material mat = settings.armorSet[armorIndex].material[index];

            sweaterMesh.material = mat;
            handsMesh.material = mat;
            pantsMesh.material = mat;
            bootsMesh.material = mat;
        }
    }
}
