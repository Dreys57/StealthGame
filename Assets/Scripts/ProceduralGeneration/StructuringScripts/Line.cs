using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
   private Orientation orientation;

   public Orientation Orientation
   {
      get => orientation;
      set => orientation = value;
   }

   public Vector2Int Coordinates
   {
      get => coordinates;
      set => coordinates = value;
   }

   private Vector2Int coordinates;

   public Line(Orientation orientation, Vector2Int coordinates)
   {
      this.orientation = orientation;
      this.coordinates = coordinates;
   }
}

public enum Orientation
{
   HORIZONTAL = 0,
   VERTICAL = 1
}