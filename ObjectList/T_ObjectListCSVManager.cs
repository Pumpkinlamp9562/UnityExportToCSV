using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public struct Data
{
    public string ObjectName;
    public string MeshName;
    public string MeshRenderer;
    public string SkinnedMeshRenderer;
    public string SpriteRenderer;
    public string ImageRenderer;
    public string ParticleSystem;
    public string MaterialCount;
    public string[] MaterialName;
    public string[] ShaderName;
}
public static class T_ObjectListCSVManager
{
    private static string reportDirectoryName = "Report";
    private static string reportFileName = "report.csv";
    private static string reportSeparator = ",";
    private static string[] reportHeaders = new string[10]
    {
        "Object Name",
        "Mesh Name",
        "Mesh Renderer",
        "Skinned Mesh Renderer",
        "Sprite Renderer",
        "Image Renderer",
        "Particle system",
        "Material Count",
        "Material Name",
        "Shader Name"
    };
    public static List<Data> MeshRendererData = new List<Data>();
    public static List<Data> SkinMeshRendererData = new List<Data>();
    public static List<Data> SpriteRendererData = new List<Data>();
    public static List<Data> ImageRendererData = new List<Data>();
    public static List<Data> ParticleRendererData = new List<Data>();

    #region Interaction
    static void ListDataAppendToReport(List<Data> data)
    {
        string start = "\"";

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].MaterialName.Length == 1)
            {
                string mName = "\"" + data[i].MaterialName[0] + "\"";
                string sName = "\"" + data[i].ShaderName[0] + "\"";
                string[] s = new string[10]
                {
                        data[i].ObjectName,data[i].MeshName,
                        data[i].MeshRenderer, data[i].SkinnedMeshRenderer,
                        data[i].SpriteRenderer, data[i].ImageRenderer,
                    data[i].ParticleSystem,
                        data[i].MaterialCount, mName, sName
                };
                AppendToReport(s);
                continue;
            }
            
            // There have more than one material
            // .....,"Material Name0
            string[] s0 = new string[9]
            {
                data[i].ObjectName,data[i].MeshName,
                data[i].MeshRenderer, data[i].SkinnedMeshRenderer,
                data[i].SpriteRenderer, data[i].ImageRenderer, data[i].ParticleSystem,
                data[i].MaterialCount, start + data[i].MaterialName[0]
            };
            AppendToReport(s0);

            //Material Name2
            //Material Name2"," Shader Name0
            UpdateListToReport(data[i].MaterialName, false, data[i].ShaderName[0]);
            //"Shader Name
            UpdateListToReport(data[i].ShaderName, true, "");
        }
    }
    public static void UpdateAllRendererData()
	{
        ListDataAppendToReport(MeshRendererData);
        ListDataAppendToReport(SkinMeshRendererData);
        ListDataAppendToReport(SpriteRendererData);
        ListDataAppendToReport(ImageRendererData);
        ListDataAppendToReport(ParticleRendererData);
    }
    static void UpdateListToReport(string[] listInData, bool last, string next)
	{
        for (int f = 1; f < listInData.Length; f++)
        {
            string[] finalString;
            if (f == 0)
            {
                finalString = new string[1] { "\"" + listInData[f] };
                AppendToReport(finalString);
            }

            finalString = new string[1] { listInData[f] };

            if (f + 1 != listInData.Length)
            {
                AppendToReport(finalString);
            }
            else
            {
                if(last)
                    finalString[0] += "\"";
                else
                    finalString[0] += "\"" + "," + "\"" + next;
                AppendToReport(finalString);
            }
        }
    }

    public static void AppendToReport(string[] strings)
	{
        VerifyDirectory();
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
		{
            string finalString = "";
            for(int i = 0; i< strings.Length; i++)
			{
                if (finalString != "")
				{
                    finalString += reportSeparator;
				}
                finalString += strings[i];
			}
            sw.WriteLine(finalString);
        }
        EditorApplication.Beep();
        Debug.Log("<color=green>Report updated successfully!</color>");
    }
    public static void CreateReport()
	{
        VerifyDirectory();
        using (StreamWriter sw = File.CreateText(GetFilePath()))
		{
            string finalString = "";
            for(int i = 0; i< reportHeaders.Length; i++)
			{
                if(finalString != "")
				{
                    finalString += reportSeparator;
				}
                finalString += reportHeaders[i];
			}
            sw.WriteLine(finalString);
		}
	}
	#endregion

	#region Operations
	static void VerifyDirectory()
	{
        string dir = GetDirectoryPath();
        if(!Directory.Exists(dir))
		{
            Directory.CreateDirectory(dir);
		}
	}

    static void VerifyFile()
	{
        string file = GetFilePath();
		if (!File.Exists(file))
		{
            CreateReport();
		}
	}
	#endregion

	#region Queries
	static string GetDirectoryPath()
	{
        return Application.dataPath + "/" + reportDirectoryName;
	}

    static string GetFilePath()
	{
        return GetDirectoryPath() + "/" + reportFileName;
	}

    static string GetTimeStamp()
	{
        return System.DateTime.UtcNow.ToString();
	}

    public static void AddMeshRendererData(MeshRenderer renderers) 
    {
        List<string> shaders = new List<string>();
        List<string> materials = new List<string>();
        for(int i = 0; i< renderers.sharedMaterials.Length; i++)
		{
                materials.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].name);
                shaders.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].shader.name);
        }
        //List out all the data
        Data d = new Data { 
            ObjectName = renderers.name, 
            MeshName = renderers.GetComponent<MeshFilter>().sharedMesh == null ? "" : renderers.GetComponent<MeshFilter>().sharedMesh.name, 
            MeshRenderer = "True", SkinnedMeshRenderer = "False",
            SpriteRenderer = "False", ParticleSystem = "False", ImageRenderer = "False",
            MaterialCount = renderers.sharedMaterials.Length.ToString() , 
            MaterialName = materials.ToArray(), ShaderName = shaders.ToArray()
        };

        MeshRendererData.Add(d);
    }

    public static void AddSkinMeshRendererData(SkinnedMeshRenderer renderers)
    {
        List<string> shaders = new List<string>();
        List<string> materials = new List<string>();
        for (int i = 0; i < renderers.sharedMaterials.Length; i++)
        {
            materials.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].name);
            shaders.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].shader.name);
        }
        //List out all the data
        Data d = new Data
        {
            ObjectName = renderers.name,
            MeshName = renderers.GetComponent<MeshFilter>().sharedMesh == null ? "" : renderers.GetComponent<MeshFilter>().sharedMesh.name,
            MeshRenderer = "False",
            SkinnedMeshRenderer = "True",
            SpriteRenderer = "False",
            ParticleSystem = "False",
            ImageRenderer = "False",
            MaterialCount = renderers.sharedMaterials.Length.ToString(),
            MaterialName = materials.ToArray(),
            ShaderName = shaders.ToArray()
        };

        MeshRendererData.Add(d);
    }
    public static void AddSpriteRendererData(SpriteRenderer renderers)
    {
        List<string> shaders = new List<string>();
        List<string> materials = new List<string>();
        for (int i = 0; i < renderers.sharedMaterials.Length; i++)
        {
            materials.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].name);
            shaders.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].shader.name);
        }
        //List out all the data
        Data d = new Data
        {
            ObjectName = renderers == null ? "" : renderers.name,
            MeshName = renderers.sprite == null ? "" : renderers.sprite.name,
            MeshRenderer = "False",
            SkinnedMeshRenderer = "False",
            SpriteRenderer = "True",
            ParticleSystem = "False",
            ImageRenderer = "False",
            MaterialCount = renderers.sharedMaterials.Length.ToString(),
            MaterialName = materials.ToArray(),
            ShaderName = shaders.ToArray()
        };

        MeshRendererData.Add(d);
    }

    public static void AddImageRendererData(Image renderers)
    {
        //List out all the data
        Data d = new Data
        {
            ObjectName = renderers == null ? "" : renderers.name,
            MeshName = renderers.sprite == null?"":renderers.sprite.name,
            MeshRenderer = "True",
            SkinnedMeshRenderer = "False",
            SpriteRenderer = "False",
            ParticleSystem = "False",
            ImageRenderer = "False",
            MaterialCount = "1",
            MaterialName = new string[1] { renderers.material == null ? "" : renderers.material.name },
            ShaderName = new string[1] { renderers.material == null ? "" : renderers.material.shader.name }
        };
        MeshRendererData.Add(d);
    }
    public static void AddParticleRendererData(ParticleSystemRenderer renderers)
    {
        List<string> shaders = new List<string>();
        List<string> materials = new List<string>();
        for (int i = 0; i < renderers.sharedMaterials.Length; i++)
        {
            materials.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].name);
            shaders.Add(renderers.sharedMaterials[i] == null ? "" : renderers.sharedMaterials[i].shader.name);
        }
        //List out all the data
        Data d = new Data
        {
            ObjectName = renderers.name,
            MeshName = renderers.mesh == null ? "" : renderers.mesh.name,
            MeshRenderer = "True",
            SkinnedMeshRenderer = "False",
            SpriteRenderer = "False",
            ParticleSystem = "False",
            ImageRenderer = "False",
            MaterialCount = renderers.sharedMaterials.Length.ToString(),
            MaterialName = materials.ToArray(),
            ShaderName = shaders.ToArray()
        };

        MeshRendererData.Add(d);
    }
    #endregion
}
