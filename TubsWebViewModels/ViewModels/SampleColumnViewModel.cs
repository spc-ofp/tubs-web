// -----------------------------------------------------------------------
// <copyright file="SampleColumnViewModel.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2013 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.ViewModels
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
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// ViewModel for a single column of PS-4 length samples.
    /// </summary>
    /// <remarks>
    /// Although it appears to work fine in the office with cutting edge versions of Chrome,
    /// 16GB of memory, and a well-maintaned version of Win7, not everyone is there yet.
    /// This is an attempt to limit the input to a subset of the full form in order to allow
    /// for reasonable performance.
    /// </remarks>
    public sealed class SampleColumnViewModel
    {
        // TODO: Should the VM have a method that fills up the list of
        // samples to 20?
        
        /// <summary>
        /// Constructor
        /// </summary>
        public SampleColumnViewModel()
        {
            this.Samples = new List<PurseSeineSampleViewModel>(20);
            
        }

        /// <summary>
        /// Sample header primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Trip primary key
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// Offset
        /// </summary>
        /// <remarks>
        /// Should be one of (0,20,40,60,80,100)
        /// </remarks>
        public int Offset { get; set; }

        /// <summary>
        /// UI display property derived from offset
        /// </summary>
        public string HeaderText
        {
            get
            {
                return String.Format("Samples {0} thru {1}", (this.Offset + 1), (this.Offset + 20));
            }
        }

        /// <summary>
        /// Collection of samples contained in this column
        /// </summary>
        public IList<PurseSeineSampleViewModel> Samples { get; set; }

    }
}
