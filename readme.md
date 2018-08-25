# Beat Saber - Saber Tweaks

Tweak almost everything about your sabers, including length, grip, and trail length!

Edit your tweaks in `\Beat Saber\UserData\modprefs.ini` - they're reloaded every time you start or restart a song, so use that for adjusting the grip values!

The default values are below:

```ini
[SaberTweaks]
Length=1
TrailLength=20
GripLeftPosition=0,0,0
GripLeftRotation=0,0,0
GripRightPosition=0,0,0
GripRightRotation=0,0,0
```

*If the file / settings don't exist, run your game once after installing!*

## Settings

All config is reloaded every time a song starts - so you can easily adjust things at runtime just by modifying the `modprefs.ini` file.

### Length

- **Setting**: `Length`
- **Unit**: Metres
- **Default**: `1`
- **Minimum**: `0.01`
- **Maximum**: `2`

**Setting this to anything other than 1 will disable score submissions!**

This setting is known to break many custom sabers, so use this at your own risk. A fix for this may be coming in the future!

### Trail Length

- **Setting**: `TrailLength`
- **Unit**: Integer (Whole Number)
- **Default**: `20`
- **Minimum**: `5`
- **Maximum**: `100`

Adjusts the length of the trail on the saber. This option *does not* disable score submission.

### Grip Position (Left + Right)

- **Setting**: `GripLeftPosition`, `GripRightPosition`
- **Unit**: Centimetres
- **Default**: `0,0,0`
- **Maximum**: `50` on any axis

Order of axis: `x`, `y`, `z`

- `+x` moves the saber right, EG: `20,0,0` moves the saber 20 centimetres right.
- `+y` moves the saber up, EG: `0,10,0` moves the saber 10 centimetres up.
- `+z` moves the saber forward, EG: `0,0,30` moves the saber 30 centimetres forward.

Alters the position of the left/right saber, relative to the default location. You cannot move the saber more than 50 centimetres away on any axis! This option *does not* disable score submission.

### Grip Rotation (Left + Right)

- **Setting**: `GripLeftRotation`, `GripRightRotation`
- **Unit**: Degrees (0 - 360)
- **Default**: `0,0,0`

Order of axis: `rx`, `ry`, `rz`

- `+rx` tilts the saber down, EG: `20,0,0` titls the saber 20 degrees down.
- `+ry` rotates the saber right, EG: `0,10,0` rotates the saber 10 degrees right.
- `+rz` rolls the saber counter-clockwise, EG: `0,0,30` rotates the saber 30 centimetres counter-clockwise.

Alters the rotation of the sabers. The centre of rotation is where the saber's hitbox starts, which is just after the glowing line on the handle. This option *does not* disable score submission.