using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using GLTFast;

[System.Serializable]
public class AssetJson
{
    public string name;
    public string artokenurl;
    public float artokenxsize;
    public float artokenysize;
    public string geometryurl;
    public string dataurl;
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

    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(DownloadAndParseJson());
    }

    void Update()
    {

    }

    IEnumerator DownloadAndParseJson()
    {
        Debug.Log("DownloadJson");
        UnityWebRequest request = UnityWebRequest.Get(assetPackageJsonUrl);
        yield return request.SendWebRequest();
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
            yield return request.SendWebRequest();
            asset.artoken = DownloadHandlerTexture.GetContent(request);

            // ar geometry
            request = UnityWebRequest.Get(ajs.geometryurl);
            yield return request.SendWebRequest();
            byte[] glbData = request.downloadHandler.data;

            var gltf = new GltfImport();
            yield return gltf.LoadGltfBinary(glbData);

            gltf.InstantiateMainScene(transform);

        }
    }
}

