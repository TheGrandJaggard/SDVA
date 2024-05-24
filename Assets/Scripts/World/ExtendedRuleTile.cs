using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Extended Rule Tile", menuName = "2D/Tiles/Extended Rule Tile")]
public class ExtendedRuleTile : RuleTile
{
    public string sibingGroup;
    public List<string> acceptedSiblingGroups;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case TilingRule.Neighbor.This:
                {
                    return other is ExtendedRuleTile
                        && acceptedSiblingGroups.Contains((other as ExtendedRuleTile).sibingGroup);
                }
            case TilingRule.Neighbor.NotThis:
                {
                    return !(other is ExtendedRuleTile
                        && acceptedSiblingGroups.Contains((other as ExtendedRuleTile).sibingGroup));
                }
        }

        return base.RuleMatch(neighbor, other);
    }
}
