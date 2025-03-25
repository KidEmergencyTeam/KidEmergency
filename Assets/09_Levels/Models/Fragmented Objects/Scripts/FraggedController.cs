using UnityEngine;
using System.Collections;
using System.Linq;

public class FraggedController : MonoBehaviour
{
	[Header("Fragments")]
	[Tooltip("Can frags fall off, set to false for floors and walls where the frags only are suppose to rotate")]
	public bool fragEnabled = true;
	[Tooltip("Maximum force put on fracture fragments when fragged")]
	public int forceMax = 250;
	[Tooltip("Minimun force put on fracture fragments when fragged")]
	public int forceMin = 50;
	[Tooltip("Scales fracture fragments after fragged")]
	public float fragOffScale = 1.0f;
	[Tooltip("Rotates randomly when hit (random degrees * fracture.hitPoints(0-1))")]
	public int rotateOnHit = 10;
	[Tooltip("Each fracture fragments mass")]
	public float fragMass = 0.01f;
	[Tooltip("Amount decreased from fracture fragment hitPoints(1 =100%) on mouse over or *collide magnitude")]
	public float hitPointDecrease = .2f;
	[Tooltip("When using Box Colliders they sometimes overlap when activated and makes fragments fly far far away, this limitation will fix that")]
	public float limitFragmentSpeed = 25.0f;
	[Tooltip("Destroys the entire object on any amount of damage.")]
	public bool fragAllOnDamage;


	[Header("Collisions")]
	[Tooltip("//Fracture fragments on collision magnitude (0 disabled, 5 good, 25 max)")]
	public int collidefragMagnitude = 0;
	[Tooltip("	//Colliders in theese layers that can fragment object")]
	public LayerMask collideMask;

	[Header("Particles")]
	[Tooltip("")]
	public Vector2 fragEmitMinMax = new Vector2(2.0f, 4.0f);


	[Header("Connections")]
	[Tooltip("Everything not connected to the bottom fragments frags off")]
	public int stickyTop;
	[Tooltip("Everything not connected to the top fragments frags off")]
	public int stickyBottom;
	[Tooltip("How many fragments needs to be connected before they break appart")]
	public int connectedFragments = 3;
	[Tooltip("Size of sphere connecting fragments")]
	public float connectOverlapSphere = .5f;
	[Tooltip("Should contain layer containing fragments")]
	public LayerMask stickyMask = (LayerMask)1;

	[Header("After Fragment")]
	[Tooltip("Once hitting a gameobject of this type the collider will be disabled.")]
	public LayerMask disableMask;
	[Tooltip("Disables fracture fragments after fragged, 0 never disable. (seconds)")]
	public float disableDelay = 0.0f;
	[Tooltip("Combine frags to a single mesh.")]
	public bool combineFrags = true;
	[Tooltip("Combines all fragments to one mesh after last fragged fragment+delay [seconds/10] (performance+++) (negative/zero=don't merge)")]
	public int combineMeshesDelay = 3;

	[HideInInspector]
	public Transform startMesh;             //Original mesh used when not fragmented
	[HideInInspector]
	public ParticleSystem fragParticles;    //This will play on every frag
	[HideInInspector]
	public ParticleSystem dustParticles;    //This will play on every hit
	[HideInInspector]
	public int reCounter = 1;               //Counter for recombining fragments
	[HideInInspector]
	public GameObject combinedFrags;        //Stores combined mesh
	[HideInInspector]
	public MeshFilter[] meshFilters;
	[HideInInspector]
	public Transform fragments;

	[Header("Change Materials")]
	[Tooltip("")]
	public Material[] fragMaterials;

	[Header("Sounds")]
	[Tooltip("Sound plays on impact, scales with magnitude")]
	public AudioClip impactSound;
	[Tooltip("")]
	public float maxPitch = 1.0f;
	[Tooltip("")]
	public float minPitch = 1.0f;
	[Tooltip("")]
	public float volume = 1f;
	[Tooltip("")]
	public float minVelocity = 1f;



	/// RESET FRAGMENTED OBJECT
	public void ResetFrags() {
		if (startMesh != null) this.startMesh.GetComponent<Renderer>().enabled = true;
		fragParticles.Clear();
		foreach (Transform child in fragments) {
			child.GetComponent<FraggedChild>().resetMe();
		}
		if (combinedFrags != null) {
			Destroy(combinedFrags);
		}
		if (startMesh == null)
			reCombine();
	}

	public void Start() {
		//// SETUP PARTICLES
		fragParticles = transform.Find("Particles Fragment").GetComponent<ParticleSystem>();
		fragParticles.Stop();
		dustParticles = transform.Find("Particles Dust").GetComponent<ParticleSystem>();
		dustParticles.Stop();
		startMesh = transform.Find("Original Mesh");
		fragments = transform.Find("Fragments");
		meshFilters = new MeshFilter[fragments.transform.childCount];
		meshFilters = fragments.transform.GetComponentsInChildren<MeshFilter>(true);
		FindSticky();
		if (startMesh == null) CombineFrags();
		InvokeRepeating("reCombine", 1.0f, 1.0f);
		ChangeMaterials();
	}

	public void ChangeMaterials() {
		for (int m = 0; m < fragMaterials.Length; m++) {
			for (int i = 0; i < fragments.childCount; i++) {
				Renderer child = fragments.GetChild(i).GetComponent<Renderer>();
				child.sharedMaterials = fragMaterials;
			}
		}
	}
	
	public void FixedUpdate() {
		if (limitFragmentSpeed > 0 && (combinedFrags == null) && !this.startMesh.GetComponent<Renderer>().enabled) {
			for (int i = fragments.childCount; i > 0; i--) {
				FraggedChild child = fragments.GetChild(i - 1).GetComponent<FraggedChild>();
				child.SpeedCheck();
			}
		}
	}
	
	public int Compare(float first, float second) {
		return second.CompareTo(first);
	}

	public void FragAll() {
		for (int i = fragments.transform.childCount; i > 0; i--) {
			FraggedChild child = fragments.GetChild(i - 1).GetComponent<FraggedChild>();
			child.fragMe(200.0f);
		}
	}

	public void checkConnections() {
		if (stickyTop > 0 || stickyBottom > 0) {
			//set all connected to false
			FraggedChild frag = null;
			for (int i = stickyTop; i < meshFilters.Length; i++) {
				frag = meshFilters[i].GetComponent<FraggedChild>();
				frag.connected = false;
			}
			//checks connections upwards
			for (int j = stickyTop; j < meshFilters.Length; j++) {
				frag.checkConnections();
				frag = meshFilters[j].GetComponent<FraggedChild>();
			}
			// checks connections down
			for (int u = meshFilters.Length - stickyBottom - 1; u >= stickyTop; u--) {
				frag = meshFilters[u].GetComponent<FraggedChild>();
				if (!frag.fragged) {
					frag.checkConnections();
					if (!frag.connected) frag.fragMe(2.0f);
				}
			}
		}
	}
	// FIND STICKY FRAGMENTS
	public void FindSticky() {
		if (stickyTop > 0 || stickyBottom > 0) {
			//JS
			//meshFilters.Sort(meshFilters, function(g1, g2) Compare(g1.transform.position.y, g2.transform.position.y));	
			//C# + ADD using System.Linq;
			meshFilters = meshFilters.OrderByDescending(x => x.transform.position.y).ToArray();
			for (int j = 0; j < stickyTop; j++) {
				FraggedChild g = meshFilters[j].GetComponent<FraggedChild>();
				g.stickyFrag = true;
			}
			for (int i = meshFilters.Length - stickyBottom; i < meshFilters.Length; i++) {
				FraggedChild k = meshFilters[i].GetComponent<FraggedChild>();
				k.stickyFrag = true;
			}
		}
	}

	public int Contains(ArrayList l, string n) {
		for (int i = 0; i < l.Count; i++) {
			if ((l[i] as Material).name == n) {
				return i;
			}
		}
		return -1;
	}

	public void EnableRenderers() {
		foreach (Transform child in fragments) {
			child.GetComponent<Renderer>().enabled = true;
		}
	}

	//// COMBINE FRAGMENTS TO A SINGLE MESH
	public void CombineFrags() {
		if ((combinedFrags == null) && !this.startMesh.GetComponent<Renderer>().enabled) {
			combinedFrags = new GameObject();
			combinedFrags.name = "Combined Fragments";
			combinedFrags.gameObject.AddComponent<MeshFilter>();
			combinedFrags.gameObject.AddComponent<MeshRenderer>();
			if (meshFilters.Length == 0) {
				meshFilters = new MeshFilter[fragments.transform.childCount];
				meshFilters = fragments.transform.GetComponentsInChildren<MeshFilter>(true);
			}
			ArrayList materials = new ArrayList();
			ArrayList combineInstanceArrays = new ArrayList();
			for (int i = 0; i < meshFilters.Length; i++) {
				MeshFilter[] meshFilterss = meshFilters[i].GetComponentsInChildren< MeshFilter > ();
				foreach (MeshFilter meshFilter in meshFilterss) {
					MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
					FraggedChild c = meshFilter.transform.GetComponent<FraggedChild>();
					if ((c != null) && c.fragged == false || (c != null) && combineFrags) { //&& c.fragged == false) {
						c.fragged = false;
						c.hitPoints = 1.0f;

						meshFilters[i].transform.gameObject.GetComponent<Renderer>().enabled = false;
						meshFilters[i].GetComponent<Rigidbody>().isKinematic = true;
						for (int o = 0; o < meshFilter.sharedMesh.subMeshCount; o++) {
							int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[o].name);
							if (materialArrayIndex == -1) {
								materials.Add(meshRenderer.sharedMaterials[o]);
								materialArrayIndex = materials.Count - 1;
							}
							combineInstanceArrays.Add(new ArrayList());
							CombineInstance combineInstance = new CombineInstance();
							combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
							combineInstance.subMeshIndex = o;
							combineInstance.mesh = meshFilter.sharedMesh;
							(combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
						}
					}
				}
			}
			Mesh[] meshes = new Mesh[materials.Count];
			CombineInstance[] combineInstances = new CombineInstance[materials.Count];
			for (int m = 0; m < materials.Count; m++) {
				CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
				meshes[m] = new Mesh();
				meshes[m].CombineMeshes(combineInstanceArray, true, true);
				combineInstances[m] = new CombineInstance();
				combineInstances[m].mesh = meshes[m];
				combineInstances[m].subMeshIndex = 0;
			}
			combinedFrags.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			combinedFrags.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combineInstances, false, false);
			foreach (Mesh mesh in meshes) {
				mesh.Clear();
				DestroyImmediate(mesh);
			}
			MeshRenderer meshRendererCombine = combinedFrags.GetComponent<MeshFilter>().GetComponent<MeshRenderer>();
			if (meshRendererCombine == null) meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
			Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
			meshRendererCombine.materials = materialsArray;
			if (Application.isEditor && !Application.isPlaying && combinedFrags.transform.parent != transform) combinedFrags.transform.parent = transform;
		}
	}

	public void ReleaseFrags(bool editor) {
		if (combinedFrags != null) {
			for (int i = 0; i < meshFilters.Length; i++) {
				meshFilters[i].transform.gameObject.GetComponent<Renderer>().enabled = true;
			}
			Destroy(combinedFrags);
		}
	}

	public void reCombine() {
		if ((startMesh == null) || !startMesh.GetComponent<Renderer>().enabled) {
			if (combineMeshesDelay >= 0 && combinedFrags == null && reCounter > combineMeshesDelay) {
				CombineFrags();
			} else if (reCounter <= combineMeshesDelay) {
				reCounter++;
			}
		}
	}
}
