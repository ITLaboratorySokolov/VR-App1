using System;
using UnityEngine;

/// <summary>
/// Script processing collision with walls
/// - display semi transparent warning borders
/// </summary>
public class WallCollisionProcessor : MonoBehaviour
{
    /// <summary> Display wall object </summary>
    [SerializeField]
    GameObject displaywall;

    /// <summary> Player tag </summary>
    [SerializeField]
    string playerTag;

    /// <summary> Active border walls on the room </summary>
    [SerializeField]
    GameObject[] borderWalls;

    /// <summary> Padding </summary>
    float mov = 0f;

    // Start
    private void Start()
    {
        for (int i = 0; i < borderWalls.Length; i++)
        {
            // inject self to walls
            WallCollisionDetector wcd = borderWalls[i].GetComponent<WallCollisionDetector>();
            wcd.SetWCP(this);
            wcd.playerTag = playerTag;
        }

    }

    /// <summary>
    /// Set width of border wall colliders
    /// </summary>
    /// <param name="wX"> Size in x </param>
    /// <param name="wZ"> Size in z </param>
    public void SetBorderWall(float wX, float wZ)
    {
        // wall parallel to z
        for (int i = 0; i < 2; i++)
        {
            // set size
            Vector3 size = borderWalls[i].GetComponent<BoxCollider>().size; 
            borderWalls[i].GetComponent<BoxCollider>().size = new Vector3(wX, size.y, size.z);

            CorrectPosition(borderWalls[i].transform, wX, wZ);
        }

        // wall parallel to x
        for (int i = 2; i < 4; i++)
        {
            // set size
            Vector3 size = borderWalls[i].GetComponent<BoxCollider>().size;
            borderWalls[i].GetComponent<BoxCollider>().size = new Vector3(wZ, size.y, size.z);

            CorrectPosition(borderWalls[i].transform, wX, wZ);
        }
    }

    /// <summary>
    /// Correct position of border wall to sit nicely on the borders of the room
    /// </summary>
    /// <param name="wallTF"> Wall to correct </param>
    /// <param name="wX"> Width in x </param>
    /// <param name="wZ"> Width in z </param>
    private void CorrectPosition(Transform wallTF, float wX, float wZ)
    {
        // move walls to be wX/2 and wZ/2
        Vector3 f = wallTF.forward;
        // wall parallel to x
        if (wallTF.rotation.eulerAngles.y % 180 == 0)
        {
            Vector3 oldP = wallTF.position;
            wallTF.position = new Vector3(oldP.x, oldP.y, (-f.z) * (wZ / 2.0f));
        }
        // wall parallel to z
        else
        {
            Vector3 oldP = wallTF.position;
            wallTF.position = new Vector3((-f.x) * wX / 2.0f, oldP.y, oldP.z);
        }
    }

    /// <summary>
    /// Move walls position by mov
    /// </summary>
    /// <param name="mov"> Vector of movement of the wall </param>
    internal void MoveWallPosition(Vector3 center)
    {
        for (int i = 0; i < borderWalls.Length; i++)
        {
            Vector3 oldP = borderWalls[i].transform.position;
            Vector3 newP = oldP + center;

            borderWalls[i].transform.position = newP;
        }
    }

    /// <summary>
    /// Display a warning wall upon collision
    /// </summary>
    /// <param name="colider"> Wall that was collided with </param>
    /// <param name="colidee"> Player </param>
    /// <param name="keepCoords"> Which coordinates should be kept </param>
    /// <returns></returns>
    internal GameObject HandleCollision(Transform colider, Transform colidee, Vector3 keepCoords)
    {
        BoxCollider wallBox = colider.GetComponent<BoxCollider>();
        keepCoords = new Vector3(Math.Abs(keepCoords.x), Math.Abs(keepCoords.y), Math.Abs(keepCoords.z));

        // position of the to be displayed wall
        Vector3 wallpos = new Vector3(keepCoords.x * colider.position.x, keepCoords.y * colider.position.y, keepCoords.z * colider.position.z)
                           + (new Vector3((1 - keepCoords.x) * colidee.position.x, (1 - keepCoords.y) * colidee.position.y, (1 - keepCoords.z) * colidee.position.z))
                           - mov * new Vector3(keepCoords.x * colider.forward.x, keepCoords.y * colider.forward.y, keepCoords.z * colider.forward.z);

        // size of the to be displayed wall
        float wallSize = wallBox.size.z + colidee.GetComponent<SphereCollider>().radius;

        // create mesh to display
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(- wallSize, - wallSize, 0);
        vertices[1] = new Vector3(+ wallSize, - wallSize, 0);
        vertices[2] = new Vector3(- wallSize, + wallSize, 0);
        vertices[3] = new Vector3(+ wallSize, + wallSize, 0);

        Vector2[] UVs = new Vector2[4];
        UVs[0] = new Vector2(0, 0);
        UVs[1] = new Vector2(1, 0);
        UVs[2] = new Vector2(0, 1);
        UVs[3] = new Vector2(1, 1);

        int[] triangles = new int[12] { 0, 1, 3, 0, 3, 2, 0, 3, 1, 0, 2, 3 };

        // overhangs walls
        Vector3 leftWall = colider.position + colider.right.normalized * wallBox.size.x/2;
        Vector3 rightWall = colider.position - colider.right.normalized * wallBox.size.x/2;

        // cut the walls so they do not overhang on the corners
        float dL = MathF.Sqrt( (leftWall.x - wallpos.x)*(leftWall.x - wallpos.x) + (leftWall.z - wallpos.z) *(leftWall.z - wallpos.z) );
        if (dL < wallSize)
        {
            // need to cut the wall on that size
            vertices[1] = new Vector3(dL, -wallSize, 0);
            vertices[3] = new Vector3(dL, +wallSize, 0);
        }

        float dR = MathF.Sqrt((rightWall.x - wallpos.x) * (rightWall.x - wallpos.x) + (rightWall.z - wallpos.z) * (rightWall.z - wallpos.z));
        if (dR < wallSize)
        {
            // need to cut the wall on that size
            vertices[0] = new Vector3(-dR, -wallSize, 0);
            vertices[2] = new Vector3(-dR, +wallSize, 0);
        }

        m.vertices = vertices;
        m.uv = UVs;
        m.triangles = triangles;

        GameObject c = Instantiate(displaywall, wallpos, Quaternion.Euler(colider.transform.rotation.eulerAngles + displaywall.transform.rotation.eulerAngles));
        c.GetComponent<MeshFilter>().mesh = m;

        return c;
    }

    /// <summary>
    /// Delete warning wall upon player exit
    /// </summary>
    /// <param name="reaction"> Warning wall </param>
    /// <param name="colidee"> Player </param>
    internal void HandleDelete(GameObject reaction, Transform colidee)
    {
        Destroy(reaction);
    }

    /// <summary>
    /// Move warning wall with player
    /// </summary>
    /// <param name="reaction"> Warning wall </param>
    /// <param name="colider"> Wall that was collided with </param>
    /// <param name="colidee"> Player </param>
    /// <param name="keepCoords"> Which coordinates should be kept </param>
    internal void HandleStay(GameObject reaction, Transform colider, Transform colidee, Vector3 keepCoords)
    {
        keepCoords = new Vector3(Math.Abs(keepCoords.x), Math.Abs(keepCoords.y), Math.Abs(keepCoords.z));
        BoxCollider wallBox = colider.GetComponent<BoxCollider>();
        Mesh m = reaction.GetComponent<MeshFilter>().mesh;
        float wallSize = wallBox.size.z + colidee.GetComponent<SphereCollider>().radius;
        var vertices = m.vertices;

        Vector3 leftWall = colider.position + colider.right.normalized * wallBox.size.x / 2;
        Vector3 rightWall = colider.position - colider.right.normalized * wallBox.size.x / 2;

        Vector3 wallpos = new Vector3(keepCoords.x * colider.position.x, keepCoords.y * colider.position.y, keepCoords.z * colider.position.z)
           + (new Vector3((1 - keepCoords.x) * colidee.position.x, (1 - keepCoords.y) * colidee.position.y, (1 - keepCoords.z) * colidee.position.z))
           - mov * new Vector3(keepCoords.x * colider.forward.x, keepCoords.y * colider.forward.y, keepCoords.z * colider.forward.z);

        // out of the room
        Vector3 LLW = colider.InverseTransformPoint(leftWall);
        Vector3 LRW = colider.InverseTransformPoint(rightWall);
        Vector3 LWP = colider.InverseTransformPoint(wallpos);
        if (!(LWP.x < LLW.x && LWP.x > LRW.x))
            return;

        reaction.transform.position = wallpos;

        // overhangs walls
        float dL = MathF.Sqrt((leftWall.x - wallpos.x) * (leftWall.x - wallpos.x) + (leftWall.z - wallpos.z) * (leftWall.z - wallpos.z));
        if (dL < wallSize)
        {
            // need to cut the wall on that size
            vertices[1] = new Vector3(dL, -wallSize, 0);
            vertices[3] = new Vector3(dL, +wallSize, 0);

            m.vertices = vertices;
        }
        else
        {
            vertices[1] = new Vector3(wallSize, -wallSize, 0);
            vertices[3] = new Vector3(wallSize, +wallSize, 0);

            m.vertices = vertices;
        }

        float dR = MathF.Sqrt((rightWall.x - wallpos.x) * (rightWall.x - wallpos.x) + (rightWall.z - wallpos.z) * (rightWall.z - wallpos.z));
        if (dR < wallSize)
        {
            // need to cut the wall on that size
            vertices[0] = new Vector3(-dR, -wallSize, 0);
            vertices[2] = new Vector3(-dR, +wallSize, 0);

            m.vertices = vertices;
        }
        else
        {
            vertices[0] = new Vector3(-wallSize, -wallSize, 0);
            vertices[2] = new Vector3(-wallSize, +wallSize, 0);

            m.vertices = vertices;
        }
    }
}
