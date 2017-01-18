/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Windows.Media.Imaging;

namespace HaMas
{
    public class Data : PropertyNotifierBase
    {
        private BitmapImage _image = null;
        public BitmapImage Image
        {
            get
            {
                return _image ?? (_image = new BitmapImage());
            }
            set
            {
                SetField(ref _image, value);
            }
        }
    }
}
