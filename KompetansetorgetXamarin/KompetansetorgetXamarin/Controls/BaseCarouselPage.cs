﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    public class BaseCarouselPage : CarouselPage
    {
        public void GoToLogin()
        {
            new Authenticater();
        }
    }
}
