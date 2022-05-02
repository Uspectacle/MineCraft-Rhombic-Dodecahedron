using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxelData
{
    public static readonly Vector3[] boxelVerts = new Vector3[14] {

        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 3.0f),
        new Vector3(1.0f, 3.0f, 1.0f),
        new Vector3(1.0f, 3.0f, 3.0f),
        new Vector3(3.0f, 1.0f, 1.0f),
        new Vector3(3.0f, 1.0f, 3.0f),
        new Vector3(3.0f, 3.0f, 1.0f),
        new Vector3(3.0f, 3.0f, 3.0f),
        new Vector3(0.0f, 2.0f, 2.0f),
        new Vector3(4.0f, 2.0f, 2.0f),
        new Vector3(2.0f, 0.0f, 2.0f),
        new Vector3(2.0f, 4.0f, 2.0f),
        new Vector3(2.0f, 2.0f, 0.0f),
        new Vector3(2.0f, 2.0f, 4.0f),
    };

    public static readonly Vector3[] faceChecks = new Vector3[12] {

        new Vector3( 1.0f,  0.0f, -1.0f),
        new Vector3( 1.0f, -1.0f,  0.0f),
        new Vector3( 1.0f,  0.0f,  1.0f),
        new Vector3( 1.0f,  1.0f,  0.0f),
        new Vector3(-1.0f,  0.0f, -1.0f),
        new Vector3(-1.0f, -1.0f,  0.0f),
        new Vector3(-1.0f,  0.0f,  1.0f),
        new Vector3(-1.0f,  1.0f,  0.0f),
        new Vector3( 0.0f, -1.0f, -1.0f),
        new Vector3( 0.0f, -1.0f,  1.0f),
        new Vector3( 0.0f,  1.0f,  1.0f),
        new Vector3( 0.0f,  1.0f, -1.0f),
    };

    public static readonly int[,] boxelTris = new int[12, 4] {

        // -z+x -y+x +x+z +x+y -x-z -y-x +z-x -x+y -y-z -y+z +z+y -z+y
        // 0 1 2 3 2 1
        { 6,  9, 12,  4}, // -z+x
        { 9,  5,  4, 10}, // -y+x
        { 7, 13,  9,  5}, // +x+z
        {11,  7,  6,  9}, // +x+y
        { 2, 12,  8,  0}, // -x-z
        { 8,  0,  1, 10}, // -y-x
        { 3,  8, 13,  1}, // +z-x
        {11,  2,  3,  8}, // -x+y
        {12,  4,  0, 10}, // -y-z
        {13,  1,  5, 10}, // -y+z
        {11,  3,  7, 13}, // +z+y
        {11,  6,  2, 12}, // -z+y
    };

    public static readonly Vector2[] boxelUvs = new Vector2[4] {

        new Vector2(1.0f, 1.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
    };

    public static Vector3 Bend (Vector3 pos) {

        return new Vector3(pos.x, pos.y * 2 + (pos.x + pos.z) % 2, pos.z);
    }

    public static float Unbend (float faceY, float posXZ) {

        if ((posXZ % 2 == 0 && faceY < 0) || (posXZ % 2 == 1 && faceY > 0)){
            return faceY;
        }
        return 0f;
    }
    
    public static Vector3 UnbendFaceChecks (Vector3 face, Vector3 pos) {

        return pos + new Vector3(face.x, Unbend(face.y, pos.x + pos.z), face.z);

    }

}
