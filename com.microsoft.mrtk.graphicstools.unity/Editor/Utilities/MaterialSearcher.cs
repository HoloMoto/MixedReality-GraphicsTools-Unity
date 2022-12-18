using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class MaterialSearcher : EditorWindow
{
    [MenuItem("Window/Graphics Tools/Material Search by Shader")]
    static void MaterialSearch()
    {
        // OpenEditorWindow
        GetWindow<MaterialSearcher>();
    }

    string[] showMaterialName;
    private string[] showShaderName;
    private string[] showPathName;
    private string _searchShaderName;
    private Material _targetMat;
    private Dictionary<string, string> data;
    private bool _useSearchMRGTOption;
    private string _SearchOption;
    void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.Space(10);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Type SearchMaterial Shader Name");
            }

            EditorGUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                _shaderName = (ShaderName) EditorGUILayout.EnumPopup("", _shaderName);
                switch (_shaderName)
                {
                    case ShaderName.MRGTStandard:
                        _searchShaderName = "Graphics Tools/Standard";
                        break;
                    case ShaderName.URPLit:
                        _searchShaderName = "Universal Render Pipeline/Lit";
                        break;
                }
            }

            if (_shaderName == ShaderName.MRGTStandard)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                     EditorGUILayout.LabelField("Search Option");
                    _useSearchMRGTOption = EditorGUILayout.Toggle("", _useSearchMRGTOption);
                    if (_useSearchMRGTOption)
                    {
                        _MRGTparams  = (MRGTStandardShaderParams)EditorGUILayout.EnumPopup("",_MRGTparams);
                    }
                    
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Material Name");
                EditorGUILayout.LabelField("Material Path");
            }

            if (data != null)
            {
                int i = 0;
                foreach (KeyValuePair<string, string>  item in data)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        showMaterialName[i] = EditorGUILayout.TextField("", item.Key);
                        showPathName[i] = EditorGUILayout.TextField("", item.Value);
                        i += 1;
                    }
                }
            }


            EditorGUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope())
            {
                string message = new string("");
                switch (_shaderName)
                {
                    case ShaderName.MRGTStandard:
                        message = "search MRGTStandardShader";
                        break;
                    case ShaderName.URPLit:
                        message = "Search URP Default shader";
                        break;
                    case ShaderName.Custom:
                        message = "Type Shader name want to Search";
                        break;
                }

                EditorGUILayout.HelpBox(message, MessageType.Info);
            }

            if (_shaderName == ShaderName.Custom)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _searchShaderName = EditorGUILayout.TextField("", _searchShaderName);
                }
            }
        }

        if (GUILayout.Button("Search"))
        {
            data = new Dictionary<string, string>();
            ProcessComponentsInAllScenes();
            showMaterialName = new string[data.Count];
            showPathName = new string[data.Count];
        }
    }

    public ShaderName _shaderName;
    public MRGTStandardShaderParams _MRGTparams;
    public enum ShaderName
    {
        MRGTStandard, //"Graphics Tools/Standard",
        URPLit, //Universal Render Pipeline/Lit
        Custom
    }

    public enum MRGTStandardShaderParams
    {
        _AlbedoAlphaMode,
        _EnableChannnelMap,
        _EnableNormalMap,
        _EnableEmission,
        _EnableTriplanerMapping,
        _EnableSSAA,
        _DirectioanlLight,
        _SpecularHighlights,
        _SpehericalHarmonics,
        _NPR,
        _Reflections,
        _RimLight,
        _VertexColors,
        _Mode
    }

    private void ProcessComponentsInAllScenes()
    {
        var list = AssetDatabase.FindAssets("t:Material")
            .Select(AssetDatabase.GUIDToAssetPath).Select(c => AssetDatabase.LoadAssetAtPath<Material>(c))
            .Where(c => c != null);

        if (_shaderName != ShaderName.MRGTStandard|| !_useSearchMRGTOption)
        {
            foreach (var n in list)
            {
                Debug.Log(n.shader.name);
                if (n.shader.name == _searchShaderName)
                {
                    data.Add(n.name, AssetDatabase.GetAssetPath(n));
                }
            } 
        }
        else if (_useSearchMRGTOption)
        {
            foreach (var n in list)
            {
                Debug.Log(n.shader.name);
                if (n.shader.name == _searchShaderName )
                {
                    float param = n.GetFloat(_MRGTparams.ToString());
                    if (param !=0)
                    {
                        data.Add(n.name, AssetDatabase.GetAssetPath(n)); 
                    }
                }
            } 
        }
    }
}