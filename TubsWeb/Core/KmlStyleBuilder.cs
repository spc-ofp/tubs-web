// -----------------------------------------------------------------------
// <copyright file="KmlStyleBuilder.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    /*
     * This file is part of TUBS.
     *
     * TUBS is free software: you can redistribute it and/or modify
     * it under the terms of the GNU Affero General Public License as published by
     * the Free Software Foundation, either version 3 of the License, or
     * (at your option) any later version.
     *  
     * TUBS is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     * GNU Affero General Public License for more details.
     *  
     * You should have received a copy of the GNU Affero General Public License
     * along with TUBS.  If not, see <http://www.gnu.org/licenses/>.
     */
    using System;
    using System.Collections.Generic;
    using Google.Kml;
    
    public sealed class KmlStyleBuilder
    {
        public const string SHADED_DOT = "http://maps.google.com/mapfiles/kml/shapes/shaded_dot.png";
        public const string PUSHPIN_FORMAT = "http://maps.google.com/mapfiles/kml/pushpin/{0}-pushpin.png";

        public const string OPAQUE_WHITE = "ffffffff";
        public const string TRANSPARENT_WHITE = "00ffffff";

        public const float SN_SCALE = 0.72F;
        public const float SH_SCALE = 0.6F;
        public const float PUSHPIN_SCALE = 1.3F;

        public static Icon SHADED_DOT_ICON = new Icon(SHADED_DOT);

        public static string[][] STYLE_DIFFS = 
        {
            new string[]{ "_shaded_dotred", "ff0000ff" },
            new string[]{ "_shaded_dotyellow", "ff7fffff" },
            new string[]{ "_shaded_dotblue", "ffffaa55" },
            new string[]{ "_shaded_dotwhite", "ffffffff" },
            new string[]{ "_shaded_dotgreen", "ff00ff55" },
            new string[]{ "_shaded_dotbrown", "ff0055aa" },
            new string[]{ "_shaded_dotblack", "ff000000" },
        };

        public static List<string> PUSHPIN_COLORS = new List<string>()
        {
            "red",
            "ylw",
            "blue",
            "wht",
            "grn",
            "ltblu",
            "blk"
        };
        
        public static List<AbstractStyleSelector> BuildStyles()
        {
            List<AbstractStyleSelector> styles = new List<AbstractStyleSelector>();
            // Styles for each activity
            foreach (string[] styleDiff in STYLE_DIFFS)
            {
                styles.Add(new Style()
                {
                    id = "sn" + styleDiff[0],
                    IconStyle = new IconStyle()
                    {
                        color = new Color(styleDiff[1]),
                        scale = SN_SCALE,
                        Icon = SHADED_DOT_ICON
                    },
                    LabelStyle = new LabelStyle()
                    {
                        color = new Color(OPAQUE_WHITE)
                    }
                });

                styles.Add(new Style()
                {
                    id = "sh" + styleDiff[0],
                    IconStyle = new IconStyle()
                    {
                        color = new Color(styleDiff[1]),
                        scale = SH_SCALE,
                        Icon = SHADED_DOT_ICON
                    },
                    LabelStyle = new LabelStyle()
                    {
                        color = new Color(TRANSPARENT_WHITE)
                    }
                });

                // Add a style map that correlates the two new n(ormal) and h(ighlight)
                // styles
                StyleMap sm = new StyleMap("msn" + styleDiff[0]);
                sm.Pairs.Add(new StyleMap.StyleMapEntry()
                {
                    Key = StyleMap.StyleState.Normal,
                    StyleUrl = "#sn" + styleDiff[0]
                });
                sm.Pairs.Add(new StyleMap.StyleMapEntry()
                {
                    Key = StyleMap.StyleState.Highlight,
                    StyleUrl = "#sh" + styleDiff[0]
                });
                styles.Add(sm);
            }

            // Add pushpins
            foreach (string pushpinColor in PUSHPIN_COLORS)
            {
                Icon pushpinIcon = new Icon(String.Format(PUSHPIN_FORMAT, pushpinColor));
                string snId = String.Format("sn_{0}pushpinNormal", pushpinColor);
                styles.Add(new Style()
                {
                    id = snId,
                    IconStyle = new IconStyle()
                    {
                        id = "mystyle",
                        scale = PUSHPIN_SCALE,
                        Icon = pushpinIcon
                    },
                    LabelStyle = new LabelStyle()
                    {
                        color = new Color(OPAQUE_WHITE)
                    }
                });

                string shId = String.Format("sn_{0}pushpinHighlight", pushpinColor);
                styles.Add(new Style()
                {
                    id = shId,
                    IconStyle = new IconStyle()
                    {
                        id = "mystyle",
                        scale = PUSHPIN_SCALE,
                        Icon = pushpinIcon
                    },
                    LabelStyle = new LabelStyle()
                    {
                        color = new Color(TRANSPARENT_WHITE)
                    }
                });

                // Add a style map for the two pushpin styles
                StyleMap sm = new StyleMap(String.Format("msn_{0}pushpin", pushpinColor));
                sm.Pairs.Add(new StyleMap.StyleMapEntry()
                {
                    Key = StyleMap.StyleState.Normal,
                    StyleUrl = "#" + snId
                });
                sm.Pairs.Add(new StyleMap.StyleMapEntry()
                {
                    Key = StyleMap.StyleState.Highlight,
                    StyleUrl = "#" + shId
                });
                styles.Add(sm);
            }

            // Add line styles
            // Normal VMS track
            styles.Add(new Style()
            {
                id = "vms_track",
                LineStyle = new LineStyle()
                {
                    color = new Color("ff77fff5"),
                    width = 1.5F
                }
            });

            // Highlighted VMS track
            styles.Add(new Style()
            {
                id = "vms_track_hl",
                LineStyle = new LineStyle()
                {
                    color = new Color("ff00aaa5"),
                    width = 1.5F
                }
            });


            StyleMap vmsStyleMap = new StyleMap("msn_vms");
            vmsStyleMap.Pairs.Add(new StyleMap.StyleMapEntry()
            {
                Key = StyleMap.StyleState.Normal,
                StyleUrl = "#vms_track"
            });
            vmsStyleMap.Pairs.Add(new StyleMap.StyleMapEntry()
            {
                Key = StyleMap.StyleState.Highlight,
                StyleUrl = "#vms_track_hl"
            });
            styles.Add(vmsStyleMap);
            return styles;
        }
    }
}