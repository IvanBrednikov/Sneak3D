using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeVines : MonoBehaviour
{
    [SerializeField]
    Terrain terrain;
    GameObject vinesObject;
    GameObject[] vines;
    [SerializeField]
    int vinesPerTree = 1;
    [SerializeField]
    float maxLength = 10f;
    [SerializeField]
    AnimationCurve vineForm = AnimationCurve.Constant(0, 1, 0);
    [SerializeField]
    int[] missPrototypesIndexes;
    [SerializeField]
    float maxPointHeightUpper;
    [SerializeField]
    float minPointHeightUpper;
    [SerializeField]
    float maxPointHeightLower;
    [SerializeField]
    float minPointHeightLower;
    [SerializeField]
    bool enableMultipleTreeUse;
    [SerializeField]
    int stepsCount = 10;
    [SerializeField]
    int cylinderCorners = 8;
    [SerializeField]
    float cylinderRadius = 0.1f;
    [SerializeField]
    float lowerExtremum = 2;
    [SerializeField]
    Material material;


    float positionKoef = 1000f;

    TreeInstance[] trees;

    public void SpawnVines()
    {
        trees = terrain.terrainData.treeInstances;
        vines = new GameObject[trees.Length * vinesPerTree];
        vinesObject = new GameObject();
        vinesObject.name = "Vines";

        int vineIndex = 0;
        for (int i = 0; i < trees.Length; i++)
        {
            TreeInstance tree = trees[i];

            //miss prototypeCheck
            bool isNotException = true;
            if (missPrototypesIndexes.Length != 0)
            {

                for (int j = 0; j < missPrototypesIndexes.Length; j++)
                    if (tree.prototypeIndex == missPrototypesIndexes[j])
                        isNotException = false;
            }

            if (isNotException)
            {
                List<int> usedTreesIndex = new List<int>();

                for (int j = 0; j < vinesPerTree; j++)
                {
                    int unusedTree = GetUnusedNearestTree(i, usedTreesIndex.ToArray());
                    if(unusedTree != -1)
                    {
                        //choosing lower or upper
                        //getting points and their height
                        int isUpper = Random.Range(0, 2);
                        float pointHeight;
                        if (isUpper == 1)
                            pointHeight = Random.Range(minPointHeightUpper, maxPointHeightUpper);
                        else
                            pointHeight = Random.Range(minPointHeightLower, maxPointHeightLower);
                        
                        Vector3 firstPosition = tree.position * positionKoef;
                        float terrainHeight = terrain.SampleHeight(firstPosition);
                        firstPosition.y = pointHeight + terrainHeight;
                        

                        isUpper = Random.Range(0, 2);
                        if(isUpper == 1)
                            pointHeight = Random.Range(minPointHeightUpper, maxPointHeightUpper);
                        else
                            pointHeight = Random.Range(minPointHeightLower, maxPointHeightLower);
                        
                        Vector3 secondPosition = trees[unusedTree].position * positionKoef;
                        terrainHeight = terrain.SampleHeight(secondPosition);
                        secondPosition.y = pointHeight + terrainHeight;

                        //creating vines
                        vines[vineIndex] = CreateVine(firstPosition, secondPosition);
                        vines[vineIndex].transform.parent = vinesObject.transform;
                        //vines[vineIndex].name = "Vine " + vineIndex;
                        vineIndex++;

                        usedTreesIndex.Add(unusedTree);
                    }
                }
            }
        }
    }

    private int GetUnusedNearestTree(int index, int[] exceptIndexes)
    {
        int result = -1;

        TreeInstance tree = trees[index];

        for (int i = 0; i < trees.Length; i++)
        {
            //self check tree
            if (index != i)
            {
                //miss prototypeCheck
                bool isNotException = true;
                if (missPrototypesIndexes.Length != 0)
                {
                    for (int j = 0; j < missPrototypesIndexes.Length; j++)
                        if (trees[i].prototypeIndex == missPrototypesIndexes[j])
                            isNotException = false;
                }

                if (isNotException)
                {
                    //distance check
                    float distance = Vector3.Distance(tree.position * positionKoef, trees[i].position * positionKoef);
                    if (distance <= maxLength)
                    {
                        bool isUsed = false;
                        int random = Random.Range(0, 101);
                        bool localEnableMultipleUse = random <= 30;

                        //used tree check
                        if (exceptIndexes != null && !(enableMultipleTreeUse && localEnableMultipleUse))
                        {
                            for (int j = 0; j < exceptIndexes.Length; j++)
                            {
                                if (i == exceptIndexes[j])
                                    isUsed = true;
                            }
                        }

                        if (!isUsed)
                        {
                            result = i;
                            break;
                        }

                    }
                }
            }
        }

        return result;
    }

    private GameObject CreateVine(Vector3 firstPoint, Vector3 secondPoint)
    {
        GameObject vine = new GameObject();
        MeshFilter filter = vine.AddComponent<MeshFilter>();
        MeshRenderer renderer = vine.AddComponent<MeshRenderer>();
        
        vine.transform.position = firstPoint;
        Mesh mesh = new Mesh(); 

        float distance = Vector3.Distance(firstPoint, secondPoint);
        float step = distance / stepsCount;

        Vector3[] points = new Vector3[stepsCount+1];
        Vector3[] vertices = new Vector3[(stepsCount+1)*cylinderCorners];
        int[,] formatedVertices = new int[stepsCount + 1, cylinderCorners];
        int[] triangles = new int[stepsCount*cylinderCorners*2*3];
        Vector3 direction = (secondPoint - firstPoint).normalized;

        //generate massive of circles centers
        for (int i = 0; i < points.Length; i++)
        {
            float length = step * i;
            float curveX = length / distance;
            float height = vineForm.Evaluate(curveX);

            if (i == 0)
                points[i] = firstPoint;
            else
            {
                points[i] = direction * length;
                points[i].y += lowerExtremum * height;
                points[i] = vine.transform.TransformPoint(points[i]);
            }
        }

        //generate vertices around points of massive "points"
        float stepAngle = 360 / cylinderCorners;
        int verticeIndex = 0;
        for (int i = 0; i < points.Length; i++)
        {

            //making perpendicular of vector "direction"
            Quaternion directionRotation = Quaternion.AngleAxis(90, Vector3.up);
            Vector3 side1 = direction;
            Vector3 side2 = directionRotation * direction;
            Vector3 perp = Vector3.Cross(side1, side2); //perpendicular of vector "direction"
            perp = perp.normalized * cylinderRadius;

            for (int j = 0; j < cylinderCorners; j++)
            {
                Quaternion stepRotation = Quaternion.AngleAxis(stepAngle * j, direction);
                Vector3 vertice = stepRotation * perp;
                vertice = points[i] + vertice;

                vertices[verticeIndex] = vine.transform.InverseTransformPoint(vertice);
                formatedVertices[i, j] = verticeIndex;

                verticeIndex++;
            }
        }

        //generate triangles
        int triangleIndex = 0;
        for(int cirle = 0; cirle < stepsCount; cirle++)
        {
            for(int corner = 0; corner < cylinderCorners; corner++)
            {
                int[] triangle1 = new int[3];
                int[] triangle2 = new int[3];

                if(corner != cylinderCorners - 1)
                {
                    triangle1[0] = formatedVertices[cirle, corner];
                    triangle1[1] = formatedVertices[cirle, corner + 1];
                    triangle1[2] = formatedVertices[cirle + 1, corner];

                    triangle2[0] = formatedVertices[cirle, corner + 1];
                    triangle2[1] = formatedVertices[cirle + 1, corner + 1];
                    triangle2[2] = formatedVertices[cirle + 1, corner];
                }
                else
                {
                    triangle1[0] = formatedVertices[cirle, corner];
                    triangle1[1] = formatedVertices[cirle, 0];
                    triangle1[2] = formatedVertices[cirle + 1, corner];

                    triangle2[0] = formatedVertices[cirle, 0];
                    triangle2[1] = formatedVertices[cirle + 1, 0];
                    triangle2[2] = formatedVertices[cirle + 1, corner];
                }
                

                triangles[triangleIndex] = triangle1[0];
                triangleIndex++;
                triangles[triangleIndex] = triangle1[1];
                triangleIndex++;
                triangles[triangleIndex] = triangle1[2];
                triangleIndex++;
                triangles[triangleIndex] = triangle2[0];
                triangleIndex++;
                triangles[triangleIndex] = triangle2[1];
                triangleIndex++;
                triangles[triangleIndex] = triangle2[2];
                triangleIndex++;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        filter.sharedMesh = mesh;
        renderer.material = material;
        MeshCollider collider = vine.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        return vine;
    }
}
