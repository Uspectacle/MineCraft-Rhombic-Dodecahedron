using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public bool useBest;
    public bool showBest;

    public int seed;
    public BiomeAttributes biome;

    public Transform player;
    public Vector3 spawnPosition;

    public Material material;
    public BlockType[] blocktypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChuncks, VoxelData.WorldSizeInChuncks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerLastChunkCoord;
    ChunkCoord playerChunkCoord;

    private void Start() {

        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChuncks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight - 50f , (VoxelData.WorldSizeInChuncks * VoxelData.ChunkWidth) / 2f);
        GenerateWorld ();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update() {

        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        // if (!playerChunkCoord.Equals(playerLastChunkCoord)){
        //     CheckViewDistance();
        // }
        playerLastChunkCoord = playerChunkCoord;
    }

    void GenerateWorld () {

        for (int x = (VoxelData.WorldSizeInChuncks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChuncks / 2) + VoxelData.ViewDistanceInChunks; x++) {
            for (int z = (VoxelData.WorldSizeInChuncks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChuncks / 2) + VoxelData.ViewDistanceInChunks; z++) {

                CreateNewChunk (x, z);
            }
        }

        player.position = spawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos) {

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new ChunkCoord(x, z);
    }

    void CheckViewDistance () {

        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x ++) {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z ++) {
                
                if (IsChunkInWorld (new ChunkCoord (x, z))) {

                    if (chunks[x, z] == null) {
                        CreateNewChunk(x, z);
                    }
                    else if (!chunks[x, z].IsActive) {
                        chunks[x, z].IsActive = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++) {

                    if (previouslyActiveChunks[i].Equals (new ChunkCoord(x, z))){
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks) {
            chunks[c.x, c.z].IsActive = false;
            
        }
    }

    void CreateNewChunk (int x, int z){

        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x, z));

    }
    public void GetCoordFromPos (Vector3 pos, out Vector3Int boxel, out Vector3 posInBoxel) {

        if (useBest) {
            boxel = new Vector3Int(Mathf.FloorToInt (pos.x / 2) * 2, Mathf.FloorToInt (pos.y / 2), Mathf.FloorToInt (pos.z / 2) * 2);
            posInBoxel = new Vector3 (pos.x - boxel.x, pos.y - boxel.y * 2, pos.z - boxel.z);

            Vector3Int shift = new Vector3Int();
            bool parity = false;

            for (shift.x = -1; shift.x < 2; shift.x++) {
                for (shift.y = -1; shift.y < 2; shift.y++) {
                    for (shift.z = -1; shift.z < 2; shift.z++) {

                        if (parity || CheckInBoxel(posInBoxel + shift)){
                            posInBoxel += -shift;
                            boxel += shift;
                            if (shift.y > 0) {
                                boxel.y += -1;
                            }
                            return;
                        }
                        parity = !parity;
                    }
                }
            }
            Debug.Log("ERROR : Boxel Not Detected");
            return;
        }
        else {
            boxel = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
            posInBoxel = new Vector3(pos.x - boxel.x, pos.y - boxel.y, pos.z - boxel.z);
            return;
        }
    }

    public BlockType BlockFromCoord (Vector3Int boxel) {
        int xChunk = boxel.x / VoxelData.ChunkWidth;
        int zChunk = boxel.z / VoxelData.ChunkWidth;


        boxel.x -= (xChunk * VoxelData.ChunkWidth);
        boxel.z -= (zChunk * VoxelData.ChunkWidth);

        return blocktypes[chunks[xChunk, zChunk].voxelMap[boxel.x, boxel.y, boxel.z]];
    }

    public bool CheckForVoxel (Vector3 pos) {

        Vector3Int check;
        Vector3 posInBoxel;

        GetCoordFromPos(pos, out check, out posInBoxel);
        
        BlockType block = BlockFromCoord(check);

        if (useBest) {
            return block.isSolid; //block.PositionIsSolid(posInBoxel);
        }
        else {
            return block.isSolid;
        }
    }
    
    public bool CheckInBoxel (Vector3 pos) {

        float x = pos.x - 1f;
        float y = pos.y - 1f;
        float z = pos.z - 1f;


        return  Mathf.Abs(x) + Mathf.Abs(y) < 1f &&
                Mathf.Abs(y) + Mathf.Abs(z) < 1f &&
                Mathf.Abs(z) + Mathf.Abs(x) < 1f;
    }

    public void BoxelFromPosition (Vector3 pos, out Vector3Int boxel, out Vector3 posInBoxel) {

        boxel = new Vector3Int(Mathf.FloorToInt (pos.x / 2) * 2, Mathf.FloorToInt (pos.y / 2), Mathf.FloorToInt (pos.z / 2) * 2);

        posInBoxel = new Vector3 (pos.x - boxel.x, pos.y - boxel.y * 2, pos.z - boxel.z);

        Vector3Int shift = new Vector3Int();

        bool parity = false;

        for (shift.x = -1; shift.x < 2; shift.x++) {
            for (shift.y = -1; shift.y < 2; shift.y++) {
                for (shift.z = -1; shift.z < 2; shift.z++) {

                    if (parity || CheckInBoxel(posInBoxel + shift)){
                        posInBoxel += -shift;
                        boxel += shift;
                        if (shift.y > 0) {
                            boxel.y += -1;
                        }
                        return;
                    }
                    parity = !parity;
                }
            }
        }
        Debug.Log("ERROR : Boxel Not Detected");
        return;
    }

    public byte GetVoxel (Vector3 pos) {

        int yPos = Mathf.FloorToInt(pos.y);

        if (useBest) {
            yPos = Mathf.FloorToInt(BoxelData.Bend(pos).y);
        }


        /* IMMUTABLE PASS */

        // If outside world, return air.
        if (!IsVoxelInWorld(pos)){
            return 0;
        }

        // If bottom block of chunk, return bedrock.
         if (yPos == 0) {
            return 1;
        }

        /* BASIC TERRAIN PASS */

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        byte voxelValue = 0;

        if (yPos == terrainHeight) {
            voxelValue = 3;
        }
        else if (yPos < terrainHeight && yPos > terrainHeight - 4) {
            voxelValue = 5;
        }
        else if (yPos > terrainHeight) {
            return 0;
        }
        else {
            voxelValue = 2;
        }
        
        /* SECOND PASS */

        if (voxelValue == 2) {
            
            foreach (Lode lode in biome.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight) {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold)) {
                        voxelValue = lode.blockID;
                    }
                }
            }
        }
        return voxelValue;
    }

    bool IsChunkInWorld (ChunkCoord coord) {
    
        if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChuncks - 1 &&
            coord.z > 0 && coord.z < VoxelData.WorldSizeInChuncks - 1) {
            return true;
        }
        else {
            return false;
        }
    }

    bool IsVoxelInWorld (Vector3 pos) {
        
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels &&
            pos.y >= 0 && pos.y < VoxelData.ChunkHeight &&
            pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels &&
            !useBest) {
            return true;
        }
        else if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels &&
                 pos.y >= 0 && BoxelData.Bend(pos).y < VoxelData.ChunkHeight &&
                 pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels &&
                 useBest) {
            return true;
        }
        else {
            return false;
        }
    }
}

[System.Serializable]
public class BlockType {

    public string blockName;
    public bool isSolid;
    public bool topIsSolid;

    public bool PositionIsSolid (Vector3 pos) {
        if (!isSolid) {
            return false;
        }
        if (pos.y > 1.5f && !topIsSolid) {
            return false;
        }
        return true;
    }

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right

    public int GetTextureID (int faceIndex) {

        switch (faceIndex) {

            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }

    [Header("Best Texture Values")]
    public int topXFrontTexture; // +x+y
    public int topXBackTexture; // -x+y
    public int topZFrontTexture; // +z+y
    public int topZBackTexture; // -z+y
    public int bottomXFrontTexture; // -y+x
    public int bottomXBackTexture; // -y-x
    public int bottomZFrontTexture; // -y+z
    public int bottomZBackTexture; // -y-z
    public int XFrontZFrontTexture; // +x+z
    public int XFrontZBackTexture; // -z+x 
    public int XBackZFrontTexture; // +z-x
    public int XBackZBackTexture; // -x-z

    // -z+x -y+x +x+z +x+y -x-z -y-x +z-x -x+y -y-z -y+z +z+y -z+y

    public int GetBestTextureID (int faceIndex) {

        switch (faceIndex) {

            case 0:
                return XFrontZBackTexture;
            case 1:
                return bottomXFrontTexture;
            case 2:
                return XFrontZFrontTexture;
            case 3:
                return topXFrontTexture;
            case 4:
                return XBackZBackTexture;
            case 5:
                return bottomXBackTexture;
            case 6:
                return XBackZFrontTexture;
            case 7:
                return topXBackTexture;
            case 8:
                return bottomZBackTexture;
            case 9:
                return bottomZFrontTexture;
            case 10:
                return topZFrontTexture;
            case 11:
                return topZBackTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}