using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;

public class MyPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            Debug.Log("Reimported Asset: " + str);
            if (str.Contains(".svg"))
            {
                //SvgLayerImporter(str);
            }
        }

        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }



    static void SvgLayerImporter(string assetPath)
    {
        var originalSvgPathFull = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + assetPath;

        var sceneInfo = SVGParser.ImportSVG(File.OpenText(originalSvgPathFull));
        if (null == sceneInfo.Scene)
        {
            Debug.LogError("Could not parse SVG at " + originalSvgPathFull + "!");
        }
        else
        {
            Debug.Log("Custom import: " + originalSvgPathFull);

            // this could probably stand to be specified elsewhere
            var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 100.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

            // duplicate the original SVG prefab and create a GameObject that will serve as a pretend "instance" of that prefab
            var newPrefabLocation = assetPath.Replace(".svg", "_Split.prefab");
            //var ob = new GameObject("111");
            //PrefabUtility.SaveAsPrefabAsset(ob, newPrefabLocation);

            var newSceneInfo = SVGParser.ImportSVG(File.OpenText(originalSvgPathFull));
            var geom = VectorUtils.TessellateScene(newSceneInfo.Scene, tessOptions);
            var newSprite = VectorUtils.BuildSprite(geom, 10.0f, VectorUtils.Alignment.SVGOrigin, Vector2.zero, 128, true);

            var ob = new GameObject("111");
            SpriteRenderer spriteRenderer = ob.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = newSprite;

            //PrefabUtility.CreatePrefab(newPrefabLocation, ob);
            //PrefabUtility.SaveAsPrefabAsset(ob, newPrefabLocation);
            //AssetDatabase.ImportAsset(newPrefabLocation);
            //AssetDatabase.
            //Debug.Log(newSprite.bounds);
            //AssetDatabase.AddObjectToAsset(newSprite, newPrefabLocation);
            //AssetDatabase.SaveAssets();

            //AssetDatabase.CreateAsset(newSprite, assetPath.Replace(".svg", "_test.prefab"));
            //AssetDatabase.SaveAssets();

            //PrefabUtility.CreatePrefab(assetPath.Replace(".svg", "_test.prefab"), ob);
            ////AssetDatabase.CreateAsset(ob, assetPath.Replace(".svg", "_test.prefab"));
            //AssetDatabase.AddObjectToAsset(newSprite, assetPath.Replace(".svg", "_test.prefab"));
            //AssetDatabase.SaveAssets();
            //Sprite t = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath.Replace(".svg", "_test.prefab"), typeof(Sprite));
            //Debug.Log(t.bounds);

            //AssetDatabase.AddObjectToAsset(newSprite, ob);
            //PrefabUtility.SaveAsPrefabAsset(ob, newPrefabLocation);

            //PrefabUtility.CreateEmptyPrefab(newPrefabLocation);
            //var newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabLocation);
            //Debug.Log(newPrefab);
            //var instPrefab = new GameObject();

            /*
            // go through each layer of the SVG, and in each layer, remove SVG info nodes that are *not* in that layer
            int numOriginalLayers = sceneInfo.Scene.Root.Children.Count;
            for (int i = 0; i < numOriginalLayers; ++i)
            {
                var newSceneInfo = SVGParser.ImportSVG(File.OpenText(originalSvgPathFull));
                int removed = 0;
                var layerName = "";
                for (int c = 0; c < numOriginalLayers; ++c)
                {
                    if (c == i)
                    {
                        layerName = "Layer " + c;
                    }
                    else
                    {
                        newSceneInfo.Scene.Root.Children.Remove(newSceneInfo.Scene.Root.Children[c - removed]);
                        ++removed;
                    }
                }

                // instantiate the current layer as a new sprite in the scene, and attach it to our new prefab which will soon be saved to disk
                var geom = VectorUtils.TessellateScene(newSceneInfo.Scene, tessOptions);
                var newSprite = VectorUtils.BuildSprite(geom, 10.0f, VectorUtils.Alignment.SVGOrigin, Vector2.zero, 128, true);
                newSprite.name = layerName;

                // each SVG layer will have a corresponding GameObject that's part of the new prefab
                GameObject go = new GameObject(layerName);
                SpriteRenderer s = go.AddComponent<SpriteRenderer>();
                s.sprite = newSprite;
                go.transform.SetParent(instPrefab.transform);

                // bundle the new sprite (for this layer) in with the new SVG prefab
                AssetDatabase.AddObjectToAsset(newSprite, newPrefab);
            }

            // now apply the changes from the instantiated prefab to the saved prefab on disk, and destroy the instance
            PrefabUtility.ReplacePrefab(instPrefab, newPrefab, ReplacePrefabOptions.ReplaceNameBased);
            Object.DestroyImmediate(instPrefab);
            */
        }

        return;
        
    }

    static void SvgLayerImporterModified(string assetPath)
    {
        var originalSvgPathFull = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + assetPath;

        var sceneInfo = SVGParser.ImportSVG(File.OpenText(originalSvgPathFull));
        if (null == sceneInfo.Scene)
        {
            Debug.LogError("Could not parse SVG at " + originalSvgPathFull + "!");
        }
        else
        {
            Debug.Log("Custom import: " + originalSvgPathFull);

            var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 100.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

            var newPrefabLocation = assetPath.Replace(".svg", "_Split.prefab");
            var parentObj = new GameObject("testObject");

            int numOriginalLayers = sceneInfo.Scene.Root.Children.Count;

            var objs = new GameObject[numOriginalLayers];
            var sprites = new Sprite[numOriginalLayers];

            //for (int i = 0; i < numOriginalLayers; i++)
            //{
            //    objs[i] = new GameObject(numOriginalLayers.ToString());

            //}

            for (int i = 0; i < numOriginalLayers; i++)
            {
                var newSceneInfo = sceneInfo;
                int removed = 0;
                var layerName = "";

                for (int c = 0; c < numOriginalLayers; c++)
                {
                    if (c == i)
                    {
                        layerName = "Layer " + c;
                    }
                    else
                    {
                        if (c - removed >= 0 && c - removed < newSceneInfo.Scene.Root.Children.Count)
                        {
                            newSceneInfo.Scene.Root.Children.Remove(newSceneInfo.Scene.Root.Children[c - removed]);
                        }

                        ++removed;
                    }
                }

                // instantiate the current layer as a new sprite in the scene, and attach it to our new prefab which will soon be saved to disk
                var geom = VectorUtils.TessellateScene(newSceneInfo.Scene, tessOptions);
                sprites[i] = VectorUtils.BuildSprite(geom, 10.0f, VectorUtils.Alignment.SVGOrigin, Vector2.zero, 128, true);
                sprites[i].name = layerName;

                // each SVG layer will have a corresponding GameObject that's part of the new prefab
                //GameObject go = new GameObject(layerName);
                objs[i] = new GameObject(layerName);
                SpriteRenderer s = objs[i].AddComponent<SpriteRenderer>();
                s.sprite = sprites[i];
                objs[i].transform.SetParent(parentObj.transform);
                Debug.Log(objs[i].GetComponent<SpriteRenderer>().sprite.bounds);
                Debug.Log(sprites[i].name + sprites[i].bounds);

                //AssetDatabase.AddObjectToAsset(sprites[i].texture, objs[i]);
                var tempPath = assetPath.Replace(".svg", "_" + i.ToString() + ".prefab");
                PrefabUtility.SaveAsPrefabAsset(objs[i], tempPath);

                AssetDatabase.ImportAsset(tempPath);
                AssetDatabase.AddObjectToAsset(sprites[i], objs[i]);
                AssetDatabase.SaveAssets();
            }

            // now apply the changes from the instantiated prefab to the saved prefab on disk, and destroy the instance
            Debug.Log(parentObj.transform.childCount);


            PrefabUtility.SaveAsPrefabAsset(parentObj, newPrefabLocation);
            //PrefabUtility.SaveAsPrefabAsset(instPrefab, newPrefabLocation);

            for (int i = 0; i < objs.Length; i++)
            {
                Debug.Log(objs[i].name);
                Debug.Log(objs[i].GetComponent<SpriteRenderer>().sprite.bounds);
            }

            Object.DestroyImmediate(parentObj);
        }

        return;
    }


}
