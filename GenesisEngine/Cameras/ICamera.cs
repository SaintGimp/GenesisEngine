using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface ICamera
    {
        void Reset();

        void SetViewParameters(DoubleVector3 location, float yaw, float pitch, float roll);

        void SetViewParameters(DoubleVector3 location, DoubleVector3 lookAt);
        
        void SetProjectionParameters(float fieldOfView, float zoomLevel, float aspectRatio, float nearPlane, float farPlane);
        
        void SetClippingPlanes(float nearPlane, float farPlane);

        float ZoomLevel { get; set; }

        float Yaw { get; set; }
        
        float Pitch { get; set; }
        
        float Roll { get; set; }
        
        DoubleVector3 Location { get; set; }

        void ChangeYaw(float amount);
        
        void ChangePitch(float amount);

        void MoveForwardHorizontally(double distance);
        
        void MoveBackwardHorizontally(double distance);
        
        void MoveLeft(double distance);
        
        void MoveRight(double distance);
        
        void MoveUp(double distance);
        
        void MoveDown(double distance);

        Matrix OriginBasedViewTransformation { get; }
        
        Matrix ProjectionTransformation { get; }

        BoundingFrustum OriginBasedViewFrustum { get; }
    }
}
