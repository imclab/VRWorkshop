﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Interface for all tools.
// NOTE: This file is a work in progress, changes to come.
public class ToolModel : MonoBehaviour {

  public float easing = 0.5f;

  private Tool tool_;
  private ToolController controller_;

  public Tool GetLeapTool() {
    return tool_;
  }

  public void SetLeapTool(Tool tool) {
    tool_ = tool;
  }

  public ToolController GetController() {
    return controller_;
  }

  public void SetController(ToolController controller) {
    controller_ = controller;
  }
  
  public Quaternion GetToolRotation() {
    Quaternion local_rotation = Quaternion.FromToRotation(Vector3.forward,
                                                          tool_.Direction.ToUnity());
    return GetController().transform.rotation * local_rotation;
  }

  public Vector3 GetToolTipVelocity() {
    Vector3 local_point = tool_.TipVelocity.ToUnityScaled();
    return GetController().transform.TransformDirection(local_point);
  }

  public Vector3 GetToolTipPosition() {
    Vector3 local_point = tool_.TipPosition.ToUnityScaled();
    return GetController().transform.TransformPoint(local_point);
  }
  
  public Vector3 GetToolCenter() {
    Vector3 local_point = tool_.TipPosition.ToUnityScaled() - 
                          (tool_.Length * tool_.Direction).ToUnityScaled() / 2;
    return GetController().transform.TransformPoint(local_point);
  }

  public void InitTool() {
    transform.position = GetToolCenter();
    transform.rotation = GetToolRotation();
  }

  public void UpdateTool() {
    Vector3 target_position = GetToolCenter();
    rigidbody.velocity = (target_position - transform.position) *
                         (1 - easing) / Time.fixedDeltaTime;

    // Set angular velocity.
    Quaternion target_rotation = GetToolRotation();
    Quaternion delta_rotation = target_rotation *
                                Quaternion.Inverse(transform.rotation);
    float angle = 0.0f;
    Vector3 axis = Vector3.zero;
    delta_rotation.ToAngleAxis(out angle, out axis);

    if (angle >= 180) {
      angle = 360 - angle;
      axis = -axis;
    }
    if (angle != 0)
      rigidbody.angularVelocity = (1 - easing) * angle * axis;
  }
}
