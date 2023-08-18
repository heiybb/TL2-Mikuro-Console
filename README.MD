# TL2-Mikuro-Console

Since I am not familiar with WPF, so I decide to create a `Console` version of `TL2-Mikuro` only works for MOD packing.

## Known Issue
### D3D9RenderSystem
Sometimes the `D3D9RenderSystem::_createRenderWindow` fail to initialize, and not be able to correctly handled.  
Wait for 500ms then try again should be fine, might be related to some memory R/W permission problem.  

### MMP Issue
If there is no `.BINLAYOUT` generated for `.LAYOUT`, the `.MPP` files might not be able to generated correctly at the first build.  
Refer to this [issue](https://github.com/heiybb/TL2-Mikuro/issues/1) in `TL2-Mikuro`.  
So if you make changes in the `.LAYOUT` file, and there is `ROOM` or `Particle` chunk in `LAYOUT` file, you will need to build twice.  
Check [logic](https://github.com/heiybb/TL2-Mikuro-Console/blob/91911a13285e82e2cabb5864426cf8e1605c2dde/Program.cs#L157) for more details.  
--> Potential fix: manually generate `BINLAYOUT`


## Screenshots


