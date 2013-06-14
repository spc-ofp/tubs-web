// -----------------------------------------------------------------------
// <copyright file="GrabBagTests.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2012 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels.Tests
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
    using NUnit.Framework;

    /// <summary>
    /// GrabBagTests is used to quickly validate code snippets.
    /// Yes, it's a bad idea... 
    /// </summary>
    [TestFixture]
    public class GrabBagTests
    {
        /// <summary>
        /// C# version of the Razor helper used to format latitude values
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        internal string FormatLatitude(double? latitude) {
            if (latitude.HasValue && Math.Abs(latitude.Value) <= 90) {
                string hemisphere = latitude.Value < 0 ? "S" : "N";
                int degrees = Math.Abs(latitude.Value > 0 ? (int)Math.Floor(latitude.Value) : (int)Math.Ceiling(latitude.Value));
                //int degrees = Math.Abs((int)latitude.Value);
                double decimalMinutes = Math.Abs(latitude.Value % 1);
                double minutes = decimalMinutes * 60.0;
                return String.Format("{0:D2} {1:00.000}{2}", degrees, minutes, hemisphere);
            } else {
                return "N/A";  
            }
        }

        /// <summary>
        /// C# version of the Razor helper used to format longitude values
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        internal string FormatLongitude(double? longitude) {
            if (longitude.HasValue && Math.Abs(longitude.Value) <= 180)
            {
                string hemisphere = longitude.Value < 0 ? "W" : "E";
                int degrees = Math.Abs((int)longitude.Value);
                double decimalMinutes = Math.Abs(longitude.Value % 1);
                double minutes = decimalMinutes * 60;
                return String.Format("{0:D3} {1:00.000}{2}", degrees, minutes, hemisphere);
            }
            else
            {
                return "N/A";   
            }
        }
        
        [Test]
        public void LatitudeFormat()
        {
            var input = "0123165N";
            var result = input.Substring(0, 4) + "." + input.Substring(4);
            StringAssert.AreEqualIgnoringCase("0123.165N", result);
        }

        [Test]
        public void LongitudeFormat()
        {
            var input = "17257047E";
            var result = input.Substring(0, 5) + "." + input.Substring(5);
            StringAssert.AreEqualIgnoringCase("17257.047E", result);
        }

        [Test]
        public void RazorHelperMethods()
        {
            double? latitude = 7.15565;
            StringAssert.AreEqualIgnoringCase("07 09.339N", FormatLatitude(latitude));
            double? longitude = 171.1794;
            StringAssert.AreEqualIgnoringCase("171 10.764E", FormatLongitude(longitude));
        }
    }
}
