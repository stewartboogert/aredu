using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using GLTFast;
using System.Threading.Tasks;
using System;
using UnityEngine.Animations;

[System.Serializable]
public class AssetJson
{
    public string name;
    public string artokenurl;
    public float artokenxsize;
    public float artokenysize;
    public string geometryurl;
    public string dataurl;

    public float[] scale = { 1, 1, 1 };
    public float[] position = { 0, 0, 0 };
    public float[] rotation = { 0, 0, 0 };
}

public class Asset
{
    public Texture artoken;
}

[System.Serializable]
public class AssetPackageJson
{
    public string name;
    public string description;

    public AssetJson[] assets;
}

public class AssetPackageDownloader : MonoBehaviour
{
    public string assetPackageJsonUrl;

    private string jsonText;

    public AssetPackageJson assetPackInfo;

    private List<Asset> assets = new List<Asset>();

    private GameObject gltfGO; 

    async Task Start()
    {
        Debug.Log("Start");
        await DownloadAndParseJson();
    }

    void Update()
    {

    }

    async Task DownloadAndParseJson()
    {
        Debug.Log("DownloadJson");
        UnityWebRequest request = UnityWebRequest.Get(assetPackageJsonUrl);
        await request.SendWebRequest();
        jsonText = request.downloadHandler.text;
        Debug.Log("DownloadJson: " + jsonText);

        Debug.Log("ParseJson");
        assetPackInfo = JsonUtility.FromJson<AssetPackageJson>(jsonText);

        Debug.Log("Download assets");
        foreach (AssetJson ajs in assetPackInfo.assets)
        {
            Debug.Log("Downloading: " + ajs.name);

            // Create new asset object 
            Asset asset = new();
            assets.Append(asset);

            // Download assets to object 

            // ar token 
            request = UnityWebRequestTexture.GetTexture(ajs.artokenurl);
            await request.SendWebRequest();
            asset.artoken = DownloadHandlerTexture.GetContent(request);

            // ar geometry
            Debug.Log("GLTF importing: " + ajs.geometryurl);
            GltfImport importer = new();
            bool success = await importer.Load(new Uri(ajs.geometryurl));

            Debug.Log("GLTF Load success " + success);
            if (success)
            {
                Debug.Log("Creating game object " + ajs.name);
                gltfGO = new GameObject(ajs.name);
                Debug.Log("Scale: " + ajs.scale[0] + " " + ajs.scale[1] + " " + ajs.scale[2]);
                Debug.Log("Rotation: " + ajs.rotation[0] + " " + ajs.rotation[1] + " " + ajs.rotation[2]);
                Debug.Log("Position: " + ajs.position[0] + " " + ajs.position[1] + " " + ajs.position[2]);
                gltfGO.transform.localScale = new Vector3(ajs.scale[0], ajs.scale[1], ajs.scale[2]);
                gltfGO.transform.localRotation = Quaternion.Euler(new Vector3(ajs.rotation[0], ajs.rotation[1], ajs.rotation[2]));
                gltfGO.transform.localPosition = new Vector3(ajs.position[0], ajs.position[1], ajs.position[2]);
                gltfGO.transform.SetParent(transform, worldPositionStays: false);
                await importer.InstantiateMainSceneAsync(gltfGO.transform);

                // gltfGO.SetActive(false);
                Debug.Log("Created: " + ajs.geometryurl);
            }
        }
    }
}

