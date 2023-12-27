﻿using ProcessDoctor.Backend.Core;
using ProcessDoctor.Imaging;
using ProcessDoctor.Imaging.Extensions;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace ProcessDoctor.ViewModels;

public record ProcessViewModel(
    uint Id,
    string Name,
    string CommandLine,
    Bitmap? Image)
{
    public static ProcessViewModel Of(ProcessModel model)
        => new(
            model.Id,
            model.Name,
            model.CommandLine,
            model.ExtractAssociatedBitmap().ToAvaloniaBitmap());
}
