using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshCombiner : MonoBehaviour
{
    [SerializeField] bool addMeshCollider = true;      // 콜라이더 생성 여부
    [SerializeField] bool keepChildrenActive = true;   // 원본 활성 상태 유지
    [SerializeField] string combinedName = "CombinedMesh";

    [ContextMenu("Combine Meshes (safe)")]
    void Combine()
    {
        // 자신 포함 모든 MeshFilter 가져오기 (비활성 자식은 제외)
        var all = GetComponentsInChildren<MeshFilter>(includeInactive: false);

        // 1) 부모 자신에 붙은 MeshFilter는 제외
        var list = new List<MeshFilter>(all.Length);
        foreach (var mf in all)
        {
            if (mf == null) continue;
            if (mf.gameObject == this.gameObject) continue;      // 자신 제외
            if (mf.sharedMesh == null) continue;                  // 빈 메쉬 제외
            list.Add(mf);
        }

        if (list.Count == 0)
        {
            Debug.LogWarning("[MeshCombiner] No valid MeshFilters found.");
            return;
        }

        // 2) CombineInstance 채우기 (부모 로컬 기준 행렬)
        var combines = new CombineInstance[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            var mf = list[i];
            combines[i].mesh = mf.sharedMesh;
            combines[i].transform = transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
        }

        // 3) 새 자식 GO 생성 (같은 이름이 있으면 삭제하고 새로 만듭니다)
        var old = transform.Find(combinedName);
        if (old != null) DestroyImmediate(old.gameObject);

        var go = new GameObject(combinedName);
        go.transform.SetParent(transform, false);

        var mfNew = go.AddComponent<MeshFilter>();
        var mrNew = go.AddComponent<MeshRenderer>();

        // 4) 여러 머티리얼(서브메쉬)이 섞여 있으면 mergeSubMeshes=false 권장
        var combined = new Mesh();
        combined.name = $"{name}_Combined";
        combined.CombineMeshes(combines, mergeSubMeshes: false, useMatrices: true);
        combined.RecalculateBounds();
        // 필요시: combined.RecalculateNormals();

        mfNew.sharedMesh = combined;

        // 아무 자식의 Renderer 머티리얼 묶음을 복사(최소 1개는 존재한다고 가정)
        var srcMR = list[0].GetComponent<MeshRenderer>();
        if (srcMR != null) mrNew.sharedMaterials = srcMR.sharedMaterials;

        // 5) 콜라이더 옵션
        if (addMeshCollider)
        {
            var mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = combined;
            mc.convex = false; // 건물처럼 정적 구조물은 convex 비활성 권장
        }

        // 6) 원본 끄기 옵션
        if (!keepChildrenActive)
        {
            foreach (var mf in list)
                mf.gameObject.SetActive(false);
        }

        Debug.Log($"[MeshCombiner] Combined {list.Count} parts -> '{go.name}'");
    }
}
