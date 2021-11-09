﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Rawdata_Porfolio_2.Pages.Entity_Framework.Domain
{
    public class Episode
    {
        [Key]
        public int Id { get; set; }
        public int Title_Id { get; set; }
        public Title Title { get; set; }
        public int Season { get; set; }
        public int Ep_Number { get; set; }

        public override string ToString()
        {
            return $"Id ={Id}, Title ID = {Title_Id}, Season = {Season}, Episode Number = {Ep_Number}";
        }
    }
}
