using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GenesisEngine
{
    public class Camera : ICamera
    {
        // This is a pretty normal three-axis free-flight camera.  Left and right motion are relative
        // to the current camera facing but forward, backward, up, and down are always axis-aligned.

        ISettings _settings;

        // Attributes for view matrix
        DoubleVector3 _cameraLocation;
        float _cameraYaw;
        float _cameraPitch;
        float _cameraRoll;
        Matrix _originBasedViewMatrix;
        public const float MaximumPitch = (float) Math.PI / 2 - 0.01f;
        public const float MinimumPitch = (float) -Math.PI / 2 + 0.01f;

        // Attributes for projection matrix
        float _fieldOfView;
        float _zoomLevel;
        float _aspectRatio;
        float _nearPlane;
        float _farPlane;
        Matrix _projectionMatrix;

        // View frustum
        BoundingFrustum _originBasedViewFrustum = new BoundingFrustum(Matrix.Identity);

        public Camera(ISettings settings)
        {
            _settings = settings;
            Reset();
        }

        public void Reset()
        {
            if (_settings.CameraStartingLocation != _settings.CameraStartingLookAt)
            {
                SetViewParameters(_settings.CameraStartingLocation, _settings.CameraStartingLookAt);
            }
            else
            {
                SetViewParameters(_settings.CameraStartingLocation, 0.0f, 0.0f, 0.0f);
            }

            SetProjectionParameters(MathHelper.Pi / 4, 1.0f, 1.0f, 1.0f, (float)_settings.FarClippingPlaneDistance);
        }

        public void SetViewParameters(DoubleVector3 location, DoubleVector3 lookAt)
        {
            _cameraLocation = location;

            // To convert from a look-at vector to Euler angles, we'll go
            // via a rotation matrix.  There may be a shorter way, but
            // this works.

            Matrix rotationMatrix = CreateOriginBasedLookAt(location, lookAt);
            DoubleVector3 lookDirection = lookAt - location;

            if (IsStraightUp(lookDirection))
            {
                // singularity at north pole
                _cameraYaw = (float)Math.Atan2(rotationMatrix.M13, rotationMatrix.M33);
                _cameraPitch = MaximumPitch;
                _cameraRoll = 0;
            }
            else if (IsStraightDown(lookDirection))
            {
                // singularity at south pole
                _cameraYaw = (float)Math.Atan2(rotationMatrix.M13, rotationMatrix.M33);
                _cameraPitch = MinimumPitch;
                _cameraRoll = 0;
            }
            else
            {
                // Normal conversion
                _cameraYaw = (float)Math.Atan2(-rotationMatrix.M31, rotationMatrix.M11);
                _cameraPitch = (float)Math.Atan2(-rotationMatrix.M23, rotationMatrix.M22);
                _cameraRoll = (float)Math.Asin(rotationMatrix.M21);
            }

            ClampPitch();

            CreateViewMatrix();
        }

        bool IsStraightUp(DoubleVector3 vector)
        {
            return (vector.X == 0 && vector.Z == 0 && vector.Y > 0);
        }

        bool IsStraightDown(DoubleVector3 vector)
        {
            return (vector.X == 0 && vector.Z == 0 && vector.Y < 0);
        }

        Matrix CreateOriginBasedLookAt(DoubleVector3 location, DoubleVector3 lookAt)
        {
            Matrix rotationMatrix = Matrix.CreateLookAt(Vector3.Zero, lookAt - location, Vector3.Up);
            return rotationMatrix;
        }

        public void SetViewParameters(DoubleVector3 location, float yaw, float pitch, float roll)
        {
            _cameraLocation = location;
            _cameraYaw = yaw;
            _cameraPitch = pitch;
            _cameraRoll = roll;

            CreateViewMatrix();
        }

        public void SetProjectionParameters(float fieldOfView, float zoomLevel, float aspectRatio, float nearPlane, float farPlane)
        {
            // Set attributes for the projection matrix
            _fieldOfView = fieldOfView;
            _zoomLevel = zoomLevel;
            _aspectRatio = aspectRatio;
            _nearPlane = nearPlane;
            _farPlane = farPlane;

            CreateProjectionMatrix();
        }

        public void SetClippingPlanes(float nearPlane, float farPlane)
        {
            _nearPlane = nearPlane;
            _farPlane = farPlane;

            CreateProjectionMatrix();
        }

        public float ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                if (_fieldOfView / value < MathHelper.Pi)
                {
                    _zoomLevel = value;
                    CreateProjectionMatrix();
                }
            }
        }

        public DoubleVector3 Location
        {
            get
            {
                return _cameraLocation;
            }

            set
            {
                _cameraLocation = value;
                CreateViewMatrix();
            }
        }

        public float Yaw
        {
            get
            {
                return _cameraYaw;
            }

            set
            {
                _cameraYaw = value;
                CreateViewMatrix();
            }
        }

        public float Pitch
        {
            get
            {
                return _cameraPitch;
            }

            set
            {
                _cameraPitch = value;
                ClampPitch();
                CreateViewMatrix();
            }
        }

        public float Roll
        {
            get
            {
                return _cameraRoll;
            }

            set
            {
                _cameraRoll = value;
                CreateViewMatrix();
            }
        }

        // This is the view matrix built as though the camera were at the origin rather than at
        // its world location.  This is useful because it lets us translate meshes to be drawn
        // only as much as they need to be correct relative to the camera, not as much as they'd
        // need to be in their actual world locations (which might blow the floats they're based on).
        public Matrix OriginBasedViewTransformation
        {
            get { return _originBasedViewMatrix; }
        }

        public Matrix ProjectionTransformation
        {
            get { return _projectionMatrix; }
        }

        public BoundingFrustum OriginBasedViewFrustum
        {
            get { return _originBasedViewFrustum; }
        }

        protected void CreateProjectionMatrix()
        {
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(_fieldOfView / _zoomLevel, _aspectRatio, _nearPlane, _farPlane);

            _originBasedViewFrustum.Matrix = Matrix.Multiply(OriginBasedViewTransformation, ProjectionTransformation);
        }

        protected void CreateViewMatrix()
        {
            Matrix yawMatrix = Matrix.CreateRotationY(_cameraYaw);
            Matrix pitchMatrix = Matrix.CreateRotationX(_cameraPitch);
            Matrix rollMatrix = Matrix.CreateRotationZ(_cameraRoll);

            // In order to get the proper mouse-look behavior, where the
            // camera's up vector is always in the plane of the Y axis,
            // we need to apply pitch then yaw.
            Vector3 cameraRotation = Vector3.Transform(Vector3.Forward, pitchMatrix);
            cameraRotation = Vector3.Transform(cameraRotation, yawMatrix);
            Vector3 cameraLookAt = cameraRotation;
            Vector3 cameraUp = Vector3.Transform(Vector3.UnitY, rollMatrix);

            // Now that we have a vector pointing towards where we want the camera to look,
            // create a view matrix to represent the total transformation.
            _originBasedViewMatrix = Matrix.CreateLookAt(Vector3.Zero, cameraLookAt, cameraUp);

            // Update the view frustum since we changed the camera transforms
            _originBasedViewFrustum.Matrix = OriginBasedViewTransformation * ProjectionTransformation;
        }

        public void ChangeYaw(float amount)
        {
            Yaw += amount;
            CreateViewMatrix();
        }

        public void ChangePitch(float amount)
        {
            Pitch += amount;
            CreateViewMatrix();
        }

        private void ClampPitch()
        {
            // Constrain the pitch away from singularities at the poles
            _cameraPitch = Math.Min(_cameraPitch, MaximumPitch);
            _cameraPitch = Math.Max(_cameraPitch, MinimumPitch);
        }

        public void MoveForwardHorizontally(double distance)
        {
            distance = -distance;
            var translationVector = new DoubleVector3((distance * Math.Sin(_cameraYaw)), 0.0, (distance * Math.Cos(_cameraYaw)));
            _cameraLocation += translationVector;
            CreateViewMatrix();
        }

        public void MoveBackwardHorizontally(double distance)
        {
            var translationVector = new DoubleVector3((distance * Math.Sin(_cameraYaw)), 0.0, (distance * Math.Cos(_cameraYaw)));
            _cameraLocation += translationVector;
            CreateViewMatrix();
        }

        public void MoveLeft(double distance)
        {
            distance = -distance;
            var translationVector = new DoubleVector3((distance * Math.Sin(_cameraYaw + (Math.PI / 2))), 0.0, (distance * Math.Cos(_cameraYaw + (Math.PI / 2))));
            _cameraLocation += translationVector;
            CreateViewMatrix();
        }

        public void MoveRight(double distance)
        {
            var translationVector = new DoubleVector3((distance * Math.Sin(_cameraYaw + (Math.PI / 2))), 0.0, (distance * Math.Cos(_cameraYaw + (Math.PI / 2))));
            _cameraLocation += translationVector;
            CreateViewMatrix();
        }

        public void MoveUp(double distance)
        {
            var translationVector = new DoubleVector3(0.0, distance, 0.0);
            _cameraLocation += translationVector;
            CreateViewMatrix();
        }

        public void MoveDown(double distance)
        {
            distance = -distance;
            var translationVector = new DoubleVector3(0.0, distance, 0.0);
            _cameraLocation += translationVector;
            CreateViewMatrix();
        }
    }
}
