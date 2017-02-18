using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlobDetection : MonoBehaviour
{
    public Texture2D sourceTexture;

    private Texture2D targetTexture;


    void Start ()
    {
        targetTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGB24, false);
        for (int i = 0; i < targetTexture.width; i++)
        {
            for (int j = 0; j < targetTexture.height; j++)
            {
                targetTexture.SetPixel(i, j, new Color(0, 0, 0));
            }
        }

        detect();

        byte[] bytes = targetTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Textures/target_texture" + ".png", bytes);
    }
	
	void Update ()
    {
		
	}

    public void detect ()
    {
        Color gx, gy;
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < sourceTexture.width; i++)
        {
            for (int j = 0; j < sourceTexture.height; j++)
            {
                gx = sourceTexture.GetPixel(i + 1, j) - sourceTexture.GetPixel(i - 1, j);
                gy = sourceTexture.GetPixel(i, j + 1) - sourceTexture.GetPixel(i, j - 1);

                if (Mathf.Sqrt(gx.r * gx.r + gy.r * gy.r) > 0)
                {
                    points.Add(new Vector2(i, j));
                }
            }
        }

        List<List<Vector2>> blobs = findBlobs(points);
        for (int i = 0; i < blobs.Count; i++)
        {
            for (int j = 0; j < blobs[i].Count; j++)
            {
                targetTexture.SetPixel((int)blobs[i][j].x, (int)blobs[i][j].y, new Color(1, 1, 1));
            }
        }
    }

    private List<List<Vector2>> findBlobs (List<Vector2> points)
    {
        int currentBlobId = 0;
        List<List<Vector2>> blobs = new List<List<Vector2>>();
        while (points.Count > 0)
        {
            blobs.Add(new List<Vector2>());
            getAllNeighbors(points[0], points, blobs[currentBlobId]);
            blobs[currentBlobId].Add(points[0]);
            points.Remove(points[0]);
            for (int i = 0; i < blobs[currentBlobId].Count; i++)
            {
                points.Remove(blobs[currentBlobId][i]);
            }
            currentBlobId++;
        }
        return blobs;
    }

    private void getAllNeighbors (Vector2 point, List<Vector2> points, List<Vector2> neighbors)
    {
        neighbors.Add(point);

        for (int i = 0; i < points.Count; i++)
        {
            if (areNeighbors(point, points[i]) && !neighbors.Contains(points[i]))
            {
                getAllNeighbors(points[i], points, neighbors);
            }
        }
    }

    private bool areNeighbors(Vector2 point, Vector2 testedPoint)
    {
        if (new Vector2(point.x + 1, point.y).Equals(testedPoint) ||
            new Vector2(point.x + 1, point.y - 1).Equals(testedPoint) ||
            new Vector2(point.x, point.y - 1).Equals(testedPoint) ||
            new Vector2(point.x - 1, point.y - 1).Equals(testedPoint) ||
            new Vector2(point.x - 1, point.y).Equals(testedPoint) ||
            new Vector2(point.x - 1, point.y + 1).Equals(testedPoint) ||
            new Vector2(point.x, point.y + 1).Equals(testedPoint) ||
            new Vector2(point.x + 1, point.y + 1).Equals(testedPoint))
        {
            return true;
        }
        return false;
    }
}
