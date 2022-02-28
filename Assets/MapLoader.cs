using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * The generic map loader interface.  Map loaders should override the
 * load method to load a map from the meta data already initialized in
 * the map object passed in. They must also register themselves with
 * registerLoader for one or more Map::Types.
 *
 * @todo
 * <ul>
 *      <li>
 *          Instead of loading dungeon room data into a u4dos-style structure and converting it to
 *          an xu4 Map when it's needed, convert it to an xu4 Map immediately upon loading it.
 *      </li>
 * </ul>
 */

public class MapLoader : MonoBehaviour
{

}
