﻿using System.Numerics;

namespace DeusEngine;

public class Camera 
{
    public static Camera Main;
    //Setup the camera's location, and relative up and right directions
    public Transform transform = new Transform();
    public float AspectRatio { get; set; }
    public float Yaw { get; set; } = -90f;
    public float Pitch { get; set; }

    private float Zoom = 90f;

    public Camera(Vector3 position, Vector3 Forward, Vector3 up, float aspectRatio)
    {
        transform.Position = position;
        AspectRatio = aspectRatio;
        //transform.Forward = Forward;
        //transform.Up = up;
        //initialise the quanternion with the camera's Forward vector
        ModifyDirection(0, 0);

    }

    public static void SetMain(ref Camera NewMainCamera)
    {
        Main = NewMainCamera;
    }
    
    public void ModifyZoom(float zoomAmount)
    {
        //We don't want to be able to zoom in too close or too far away so clamp to these values
        Zoom = Math.Clamp(Zoom - zoomAmount, 1.0f, 45f);
    }

    public void ModifyScale()
    {
        
    }

    public void ModifyDirection(float xOffset, float yOffset)
    {
        Yaw += xOffset;
        Pitch -= yOffset;

        //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
        Pitch = Math.Clamp(Pitch,
            -89f,
            89f);

        var cameraDirection = Vector3.Zero;
        cameraDirection.X = MathF.Cos(DMath.DegToRad(Yaw)) * MathF.Cos(DMath.DegToRad(Pitch));
        cameraDirection.Y = MathF.Sin(DMath.DegToRad(Pitch));
        cameraDirection.Z = MathF.Sin(DMath.DegToRad(Yaw)) * MathF.Cos(DMath.DegToRad(Pitch));

        transform.Rotation = new Quaternion(cameraDirection.X,cameraDirection.Y, cameraDirection.Z, 1f);
        //Forward = Vector3.Normalize(cameraDirection);
    }

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(
            transform.Position, 
            transform.Position + transform.Forward,
            transform.Up);
    }

    public Matrix4x4 GetProjectionMatrix()
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(DMath.DegToRad(Zoom),
            AspectRatio, 
            0.1f, 
            100.0f);
    }
    
    public Quaternion GetRotation()
    {
        return Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
    }
    
    
    
}