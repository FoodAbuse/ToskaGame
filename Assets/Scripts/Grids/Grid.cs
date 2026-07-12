using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
    public interface IGrid
    {
        // change this from Parent class to Interface would be better
        //9/07 started changing this to a Interface instead of a parent class. 


        // A grid will be able to Hold a set of VECTOR2 coordinates, these will correspond to a GridSpace
        //public Dictionary<Vector2Int, GridSpace> gridSpaces;
        public Dictionary<Vector2Int, GridSpace> GridSpaces { get; }

        // this seems not to create an empty grid dictionary for child classes? maybe a Get setter is needed

        /*public Grid(int GridHeight, int GridWidth)
        {
            // here we will generate the GridSpaces
            // we will Create a GridSpace for each Vector2
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    // now we construct the gridspace for that coordinate
                    GridSpace newgridSpace =  new GridSpace(new Vector2Int(x, y));
                    //and add it to the dictionary
                    gridSpaces.Add(new Vector2Int(x,y), newgridSpace);
                }
            }
        } */


        Vector2Int GetGridSize();

        int GetGridSpaceCount();
        /*public Vector2Int GetGridSize()
        {
            // go through every key and check if its X is higher if so set maxX to that value
            if (GridSpaces != null)
            {

                int maxX = 0;
                foreach (Vector2Int v2i in GridSpaces.Keys.Where(v => v.x > maxX))
                {
                    maxX = v2i.x;
                }
                int maxY = 0;
                //same for the Y
                foreach (Vector2Int v2i in GridSpaces.Keys.Where(v => v.y > maxY))
                {
                    maxY = v2i.y;
                }
                return new Vector2Int(maxX,maxY);
            }
            else return new Vector2Int(0, 0);
        }

        public int GetGridSpaceCount()
        {
            if (GridSpaces != null)
            {
                return GridSpaces.Count;
            }
            else return 0;
        }
    */
    }
}