using EditorCustom.Attributes;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;

namespace DebugStuff
{
    internal class MeshCombiner : MonoBehaviour
    {
#if UNITY_EDITOR
        #region fields & properties
        [SerializeField] private string filePath = "Assets/Models/";
        private Transform MeshesRoot
        {
            get => useThisAsRoot ? transform : meshesRoot;
        }
        [SerializeField][DrawIf(nameof(useThisAsRoot), false)] private Transform meshesRoot;
        [SerializeField] private bool useThisAsRoot = true;
        [Tooltip("Use this if you combine meshes from the different objects")]
        [SerializeField] private bool saveObjectsWorldPosition = false;
        [SerializeField] private List<MeshFilter> meshes;
        [SerializeField][ReadOnly] private Mesh generatedMesh;
        [SerializeField][ReadOnly] private Mesh savedMesh;
        private HashSet<Material> allMaterialsCombined;
        #endregion fields & properties

        #region methods
        [Button(nameof(GetAllMeshesInChild))]
        private void GetAllMeshesInChild()
        {
            UnityEditor.Undo.RecordObject(this, "Get All Meshes");
            meshes = MeshesRoot.GetComponentsInChildren<MeshFilter>().ToList();
        }
        [Button(nameof(ReadPath))]
        private void ReadPath()
        {
            UnityEditor.Undo.RecordObject(this, "Read Path");
            string newPath = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", string.IsNullOrEmpty(filePath) ? "Assets/" : filePath, MeshesRoot.name + " Combined", "asset");
            if (string.IsNullOrEmpty(newPath)) return;
            filePath = FileUtil.GetProjectRelativePath(newPath);
        }
        /// <summary>
        /// New method combines all mateiral types
        /// </summary>
        private void CombineAllMaterials()
        {
            UnityEditor.Undo.RecordObject(this, "Combine Mesh 2");
            Vector3 lastMeshRootPosition = Vector3.zero;
            Vector3 lastMeshRootScale = Vector3.zero;
            Quaternion lastMeshRootRotation = Quaternion.identity;
            if (!saveObjectsWorldPosition)
                SaveAndResetRoot(out lastMeshRootPosition, out lastMeshRootScale, out lastMeshRootRotation);

            allMaterialsCombined = new();
            foreach (MeshFilter mesh in meshes)
            {
                if (!mesh.TryGetComponent(out Renderer meshRenderer)) continue;
                foreach (Material sharedMaterial in meshRenderer.sharedMaterials)
                {
                    allMaterialsCombined.Add(sharedMaterial);
                }
            }

            List<Mesh> subMeshes = new();
            foreach (Material material in allMaterialsCombined)
            {
                List<CombineInstance> combiners = new();
                foreach (MeshFilter filter in meshes)
                {
                    if (!filter.TryGetComponent(out MeshRenderer meshRenderer)) continue;
                    Material[] localMaterials = meshRenderer.sharedMaterials;

                    for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                    {
                        if (localMaterials[materialIndex] != material) continue;
                        CombineInstance ci = new()
                        {
                            mesh = filter.sharedMesh,
                            subMeshIndex = materialIndex,
                            transform = filter.transform.localToWorldMatrix
                        };
                        combiners.Add(ci);
                    }
                }
                Mesh mesh = new();
                mesh.CombineMeshes(combiners.ToArray(), true);
                subMeshes.Add(mesh);
            }

            List<CombineInstance> finalCombiners = new();
            foreach (Mesh mesh in subMeshes)
            {
                CombineInstance ci = new()
                {
                    mesh = mesh,
                    subMeshIndex = 0,
                    transform = Matrix4x4.identity
                };
                finalCombiners.Add(ci);
            }
            generatedMesh = new();
            generatedMesh.CombineMeshes(finalCombiners.ToArray(), false);

            if (!saveObjectsWorldPosition)
                RevertRootChanges(lastMeshRootPosition, lastMeshRootScale, lastMeshRootRotation);
        }
        private void SaveAndResetRoot(out Vector3 lastMeshRootPosition, out Vector3 lastMeshRootScale, out Quaternion lastMeshRootRotation)
        {
            lastMeshRootPosition = MeshesRoot.position;
            lastMeshRootScale = MeshesRoot.localScale;
            lastMeshRootRotation = MeshesRoot.rotation;

            MeshesRoot.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            MeshesRoot.eulerAngles = Vector3.zero;
            MeshesRoot.localScale = Vector3.one;
        }
        private void RevertRootChanges(Vector3 lastMeshRootPosition, Vector3 lastMeshRootScale, Quaternion lastMeshRootRotation)
        {
            MeshesRoot.SetPositionAndRotation(lastMeshRootPosition, lastMeshRootRotation);
            MeshesRoot.localScale = lastMeshRootScale;
        }
        [Button(nameof(CombineSaveInstantiate))]
        private void CombineSaveInstantiate()
        {
            if (meshes.Count == 0)
                GetAllMeshesInChild();
            if (meshes.Count == 0) return;

            CombineAllMaterials();
            SaveMesh(generatedMesh, true, true);
            InstantiateMesh();
        }
        private void InstantiateMesh()
        {
            if (savedMesh == null)
            {
                Debug.LogError("You must save mesh before");
                return;
            }

            GameObject newObject = new(MeshesRoot.name + " Combined");
            newObject.transform.SetParent(MeshesRoot.parent);
            newObject.transform.SetPositionAndRotation(MeshesRoot.position, MeshesRoot.rotation);
            newObject.transform.transform.localScale = MeshesRoot.localScale;

            MeshFilter filter = newObject.AddComponent<MeshFilter>();
            filter.sharedMesh = savedMesh;

            MeshRenderer newRender = newObject.AddComponent<MeshRenderer>();
            newRender.sharedMaterials = allMaterialsCombined.ToArray();
        }
        private void SaveMesh(Mesh mesh, bool makeNewInstance, bool optimizeMesh)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

            if (optimizeMesh)
                MeshUtility.Optimize(meshToSave);

            AssetDatabase.CreateAsset(meshToSave, filePath);
            AssetDatabase.SaveAssets();

            savedMesh = meshToSave;
        }
        #endregion methods
#endif //UNITY_EDITOR
    }
}