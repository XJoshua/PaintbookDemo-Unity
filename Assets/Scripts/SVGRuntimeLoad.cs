using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Experimental.U2D;

public class SVGRuntimeLoad : MonoBehaviour
{
    public Sprite[] m_Sprites;

    void Start()
    {
        //string svg =
        //    @"<svg width=""283.9"" height=""283.9"" xmlns=""http://www.w3.org/2000/svg"">
        //        <line x1=""170.3"" y1=""226.99"" x2=""177.38"" y2=""198.64"" fill=""none"" stroke=""#888"" stroke-width=""1""/>
        //        <line x1=""205.73"" y1=""198.64"" x2=""212.81"" y2=""226.99"" fill=""none"" stroke=""#888"" stroke-width=""1""/>
        //        <line x1=""212.81"" y1=""226.99"" x2=""219.9"" y2=""255.33"" fill=""none"" stroke=""#888"" stroke-width=""1""/>
        //        <line x1=""248.25"" y1=""255.33"" x2=""255.33"" y2=""226.99"" fill=""none"" stroke=""#888"" stroke-width=""1""/>
        //        <path d=""M170.08,226.77c7.09-28.34,35.43-28.34,42.52,0s35.43,28.35,42.52,0"" transform=""translate(0.22 0.22)"" fill=""none"" stroke=""red"" stroke-width=""1.2""/>
        //        <circle cx=""170.3"" cy=""226.99"" r=""1.2"" fill=""blue"" stroke-width=""0.6""/>
        //        <circle cx=""212.81"" cy=""226.99"" r=""1.2"" fill=""blue"" stroke-width=""0.6""/>
        //        <circle cx=""255.33"" cy=""226.99"" r=""1.2"" fill=""blue"" stroke-width=""0.6""/>
        //        <circle cx=""177.38"" cy=""198.64"" r=""1"" fill=""black"" />
        //        <circle cx=""205.73"" cy=""198.64"" r=""1"" fill=""black"" />
        //        <circle cx=""248.25"" cy=""255.33"" r=""1"" fill=""black"" />
        //        <circle cx=""219.9"" cy=""255.33"" r=""1"" fill=""black"" />
        //    </svg>";

        

        var tessOptions = new VectorUtils.TessellationOptions() {
            StepDistance = 100.0f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        //Pfad zur Datei
        string svgFilePath = Application.dataPath + "/Resources/testObject_layerTest-10-10.svg";

        StreamReader sr = new StreamReader(svgFilePath);
        string svgText= sr.ReadToEnd();
        print(svgText);
        sr.Close();
        sr.Dispose();

        var sceneInfo = SVGParser.ImportSVG(new StringReader(svgText));

        int NrOfLayers = sceneInfo.Scene.Root.Children.Count;

        m_Sprites = new Sprite[NrOfLayers];
        SVGParser.SceneInfo[] m_SIArray = new SVGParser.SceneInfo[NrOfLayers];
        List<VectorUtils.Geometry>[] m_Geoms = new List<VectorUtils.Geometry>[NrOfLayers];

        for (int i = 0; i < NrOfLayers; i++)
        {
            m_SIArray[i] = SVGParser.ImportSVG(new StringReader(svgText));
            int removed = 0;
            for (int c = 0; c < NrOfLayers; c++)
            {
                if (c!=i)
                {
                    //print("at " + i + " removing index " + c);
                    m_SIArray[i].Scene.Root.Children.Remove(m_SIArray[i].Scene.Root.Children[c- removed]);
                    removed++;
                }
            }

            var fullBounds = VectorUtils.SceneNodeBounds(sceneInfo.Scene.Root);
            var localBounds = VectorUtils.SceneNodeBounds(sceneInfo.Scene.Root.Children[i]);
            var pivot = localBounds.position - fullBounds.position;

            var localSceneBounds = VectorUtils.SceneNodeBounds(m_SIArray[i].Scene.Root);

            Vector2 position = new Vector2(fullBounds.position.x, fullBounds.position.y);
           // position = new Vector2(fullBounds.center.x / fullBounds.width - (localBounds.position.x) / fullBounds.width, fullBounds.center.y / fullBounds.height - localBounds.position.y / fullBounds.height);
            //position = new Vector2((fullBounds.center.x - localBounds.position.x )/ fullBounds.width, 0);
            //position = new Vector2((fullBounds.position.x-localBounds.position.x-pivot.x) / fullBounds.width , 0);
            position = new Vector2(0,0);

            print("FullBounds: " + fullBounds + " localBounds:" + localBounds);// + " localSceneBounds:"+ localSceneBounds);
            //print(i + ": " + position+" / pivot: "+pivot);

            //print(position.x * fullBounds.width + " , " + position.y * fullBounds.height);

            m_Geoms[i] = VectorUtils.TessellateScene(m_SIArray[i].Scene, tessOptions);
            m_Sprites[i] = VectorUtils.BuildSprite(m_Geoms[i], 1000.0f, VectorUtils.Alignment.TopLeft, position, 128, true);

            GameObject go = new GameObject();
            SpriteRenderer s = go.AddComponent<SpriteRenderer>();
            go.transform.parent = transform;
           // go.transform.position = new Vector3((localSceneBounds.x - fullBounds.width/2f)/1000f, (fullBounds.y + fullBounds.height/2f - localSceneBounds.y) /1000f , 0);
            go.transform.position = new Vector3((localBounds.x)/1000f, (fullBounds.y - localBounds.y) /1000f , 0);
            s.sprite = m_Sprites[i];

            //var test = typeof(VectorUtils.TessellateScene);
            //var methods = test.GetMethods();

            //// test code: generate better shape
            //bool m_BetterGeneratePhysicsShape = true;
            //if (m_BetterGeneratePhysicsShape)
            //{
            //    var test = typeof(VectorUtils);
            //    var methods = test.GetMethods();
            //    foreach (MethodInfo mf in methods)
            //    {
            //        Debug.Log(mf.Name);
            //    }

            //    var physicsShapes = VectorUtils.TraceNodeHierarchyShapes(sceneInfo.Scene.Root, tessOptions);

            //    var rect = sceneInfo.SceneViewport;

            //    foreach (var vertices in physicsShapes)
            //    {
            //        if (rect == Rect.zero)
            //        {
            //            rect = VectorUtils.Bounds(vertices);
            //            VectorUtils.RealignVerticesInBounds(vertices, rect, flip: true);
            //        }
            //        else
            //        {
            //            VectorUtils.FlipVerticesInBounds(vertices, rect);
            //            VectorUtils.ClampVerticesInBounds(vertices, rect);
            //        }
            //    }

            //    m_Sprites[i].OverridePhysicsShape(physicsShapes);
            //}
            //// test code end

            go.AddComponent<PolygonCollider2D>();

            //PrefabUtility.SaveAsPrefabAsset(go, svgFilePath.Replace(".svg", "_2" + i.ToString() + ".prefab"));
        }



        //AssetImportContext ctx;

        //ctx.AddObjectToAsset();

        //var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);

        //// Build a sprite with the tessellated geometry.
        //var sprite = VectorUtils.BuildSprite(geoms, 1000.0f, VectorUtils.Alignment.TopLeft, Vector2.zero, 128, true);
        //GetComponent<SpriteRenderer>().sprite = sprite;

        //GenerateSpriteAsset(ctx, sprite, name);

    }

    void OnDisable()
    {
        GameObject.Destroy(GetComponent<SpriteRenderer>().sprite);
    }

//    private void GenerateSpriteAsset(AssetImportContext ctx, Sprite sprite, string name)
//    {
//        sprite.name = name + "Sprite";
//        if (sprite.texture != null)
//            sprite.texture.name = name + "Atlas";

//        m_ImportingSprite = sprite;

//        // Apply GUID from SpriteRect
//#if UNITY_2018_2_OR_NEWER
//        sprite.SetSpriteID(m_SpriteData.SpriteRect.spriteID);
//#else
//            var so = new SerializedObject(sprite);
//            so.FindProperty("m_SpriteID").stringValue = m_SpriteData.SpriteRect.spriteID.ToString();
//            so.ApplyModifiedPropertiesWithoutUndo();
//#endif

//        sprite.hideFlags = HideFlags.None;

//        ctx.AddObjectToAsset("sprite", sprite);

//        Material mat = MaterialForSVGSprite(sprite);

//        var gameObject = new GameObject(name);
//        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
//        spriteRenderer.sprite = sprite;
//        spriteRenderer.material = mat;

//        SetPhysicsShape(sprite);

//        if (sprite.texture != null)
//            ctx.AddObjectToAsset("texAtlas", sprite.texture);

//        ctx.AddObjectToAsset("gameObject", gameObject);
//        ctx.SetMainObject(gameObject);
//    }


}
