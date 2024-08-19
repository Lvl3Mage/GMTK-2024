
using System;
using System.Collections;
using Lvl3Mage.CameraManagement2D;
using Lvl3Mage.InterpolationToolkit.Splines;
using UnityEngine;

public class CameraGridController : CameraController
{
	CameraState cameraState;
	
    [SerializeField] float cameraBoundsPadding;
    // [SerializeField] AnimationCurveSplineCreator splineCreator;
    [SerializeField] float control1Pos = 0.3333f;
    [SerializeField] float control2Pos = 0.6666f;
    [SerializeField] float expandDuration = 2;
	
	public IEnumerator ExpandTo(Bounds gridBounds)
	{
		// Debug.Log($"Expanding grid {gridBounds}");
		Bounds bounds = GetCameraBounds(gridBounds);
		// Debug.Log($"Expanding to bounds {bounds}");
		
		
		CameraState initialState = CameraState.FromCamera(controllerCamera);
		// Debug.Log($"Initial state {initialState.Zoom}");
		CameraState finalState = CameraState.CoveringBounds(bounds,controllerCamera.aspect);
		// Debug.Log($"Final state {finalState.Zoom}");
		
		CameraStateTransform initialTransform = CameraStateTransform.Empty;
		CameraStateTransform finalTransform = finalState - initialState;
		// Debug.Log($"Final transform {finalTransform.ZoomDelta}");
		CameraStateTransform control1 = finalTransform * control2Pos;
		// Debug.Log($"Control1 {control1.ZoomDelta}");
		CameraStateTransform control2 = finalTransform * control1Pos;
		// Debug.Log($"Control2 {control2.ZoomDelta}");
		I4PointSplineFactory splineCreator = new Spline4PointFactoryProvider(CubicSpline.Hermite);
		
		CameraStateTransform.TransformSpline spline = CameraStateTransform.CreateTransformSpline(initialTransform, control1, control2, finalTransform, splineCreator);
		float accumTime = 0;
		
		ISpline test = CubicSpline.Hermite(0, 0, 4, 4);

		while (accumTime < expandDuration)
		{
			float t = accumTime / expandDuration;
			Debug.Log($"t {t}");
			Debug.Log($"Test {spline(t).ZoomDelta}");
			CameraStateTransform currentTransform = spline(t);
			cameraState = initialState + currentTransform;
			Debug.Log($"Camera state {cameraState.Zoom}");
			accumTime += Time.deltaTime;
			yield return null;
			
		}
		cameraState = finalState;
	}
	Bounds GetCameraBounds(Bounds gridBounds)
	{
		gridBounds.Expand(new Vector3(cameraBoundsPadding*2, cameraBoundsPadding*2, 0));
		return gridBounds;
	}
	public CameraStateClamp GetCameraClamp(Bounds bounds)
	{
		bounds = GetCameraBounds(bounds);
		CameraStateClamp clamp =
			CameraStateClamp.CoveringBounds(bounds, controllerCamera.aspect, CameraStateClamp.ClampMode.ClampBounds);
		//Todo fix package
		clamp.zoomClamp = new Vector2(4, Mathf.Max(bounds.size.x, bounds.size.y));
		clamp.clampZoom = true;
		return clamp;
	}
	protected override CameraState ComputeCameraState()
	{
		return cameraState;
	}
}