using UnityEngine;

public class MeshMerger : MonoBehaviour
{
	// Start is called before the first frame update
	public void Configure(Material mat, bool destroyChildren = true, bool addCollider = true) {
		//Debug.Log($"Merging {gameObject}", this);

		if (!gameObject.GetComponent<MeshRenderer>())
			gameObject.AddComponent<MeshRenderer>();

		GetComponent<MeshRenderer>().material = mat;

		//Create a single mesh joining all children meshes
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}

		//NOTE: MeshFilter must be created after querying
		//the ComponentsInchildren to avoid
		//returning also the current EMPTY NEW mesh filter
		//We could detect if a meshfilter has empty vertices and then
		//reduce the size of the combine array or use a dynamic array/list

		MeshFilter mf;

		if (!gameObject.GetComponent<MeshFilter>()) {
			gameObject.AddComponent<MeshFilter>();
		}

		mf = GetComponent<MeshFilter>();
		
		//Create final mesh
		Mesh mesh = new Mesh();
		mesh.CombineMeshes(combine);
		mf.sharedMesh = mesh;
		transform.gameObject.SetActive(true);

		
		foreach (MeshFilter child in meshFilters) {
			if (destroyChildren) {
			//Destroy all children
				Destroy(child.gameObject);
			}
			else {
				child.gameObject.SetActive(false);
			}
		}

		//Add global collider to detect hit by player bullet
		if (addCollider) {
			BoxCollider bc = gameObject.AddComponent<BoxCollider>();
		}

		Destroy(this);
	}
}
