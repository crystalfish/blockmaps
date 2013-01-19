using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//
// TILE TYPES
//
// Each grid space can be thought of as a type of tile
//
public enum TileType
{
    // Unassigned, probably a tile next to a cladding tile
    Unassigned = -2,

    // The system's type for tiles that hold cladding mesh
    CladdingTile = -1,

    // The user's cuboid type
    Cuboid,

    // The user's triangular prism type who's right angled edge points to the North West
    TriangularPrismNW,

    // The user's triangular prism type who's right angled edge points to the North East
    TriangularPrismNE,

    // The user's triangular prism type who's right angled edge points to the South East
    TriangularPrismSE,

    // The user's triangular prism type who's right angled edge points to the South West
    TriangularPrismSW,

}