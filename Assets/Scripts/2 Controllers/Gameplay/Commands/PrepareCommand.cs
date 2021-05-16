using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeGardeners
{

    public class PrepareCommand : ICommand
    {
        private bool debug = false;

        public void Execute(GridCell cell, Tool tool, GnomeController gnome)
        {
            Log("Executing");
            var occupant = cell.Occupant;
            if (occupant != null)
            {
                Log("Occupant found!");
                var associatedObject = occupant.AssociatedObject;
                var obstacle = associatedObject.GetComponent<Obstacle>();
                if(obstacle != null )
                {
                    Log("Obstacle found!");
                    obstacle.Interact(tool);
                }
            }

            if (occupant == null && cell.GroundType.Equals(GroundType.FallowSoil))
            {
                GameManager.Instance.GridManager.ChangeTile(cell.GridPosition, GroundType.ArableSoil);
            }
        }

        private void Log(string msg)
        {
            if (debug)
                Debug.Log("[PrepareCommand]: " + msg);
        }

        private void LogWarning(string msg)
        {
            if (debug)
                Debug.LogWarning("[PrepareCommand]: " + msg);
        }
    }
}
