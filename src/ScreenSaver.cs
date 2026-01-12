using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

public class SettingsForm : Form
{
    private NumericUpDown numStarCount;
    private NumericUpDown numStarSize;
    private CheckBox chkInteractive;
    private CheckBox chkEnableSpeedIncrease;
    private NumericUpDown numSpeedInterval;
    private NumericUpDown numSpeedIncrement;
    
    // Nebula Settings
    private CheckBox chkEnableNebula;
    private FlowLayoutPanel nebulaIntervalPanel;
    private NumericUpDown numNebulaHour;
    private NumericUpDown numNebulaMinute;
    private NumericUpDown numNebulaDuration;
    private CheckBox chkResetOnToggle;

    private Button btnSave;
    private Button btnCancel;

    public SettingsForm()
    {
        this.Text = "Starfield Settings";
        this.Size = new Size(520, 700);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.AutoScroll = true;
        
        // Modern-ish Styling
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        this.BackColor = Color.White; // Clean white background

        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        var mainLayout = new TableLayoutPanel();
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.Padding = new Padding(20); // More breathing room
        mainLayout.RowCount = 5; // Header 1, Content 1, Header 2, Content 2, Buttons
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Button row
        
        // --- Appearance Section ---
        var lblHeader1 = new Label() { 
            Text = "Appearance", 
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 90, 158), // Windows Blue-ish
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 10)
        };
        
        var layoutAppearance = new TableLayoutPanel() { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(10, 0, 0, 20) };
        layoutAppearance.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        layoutAppearance.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        
        layoutAppearance.Controls.Add(new Label() { Text = "Star Density (Lower = More Stars):", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top }, 0, 0);
        numStarCount = new NumericUpDown() { Minimum = 1, Maximum = 100, Value = 8, Width = 100 };
        layoutAppearance.Controls.Add(numStarCount, 1, 0);

        layoutAppearance.Controls.Add(new Label() { Text = "Star Size:", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Padding = new Padding(0, 10, 0, 0) }, 0, 1);
        numStarSize = new NumericUpDown() { Minimum = 1, Maximum = 20, Value = 3, Width = 100, Margin = new Padding(3, 10, 3, 3) };
        layoutAppearance.Controls.Add(numStarSize, 1, 1);
        
        // --- Behavior Section ---
        var lblHeader2 = new Label() { 
            Text = "Behavior", 
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 90, 158),
            AutoSize = true,
            Padding = new Padding(0, 10, 0, 10)
        };

        var layoutBehavior = new TableLayoutPanel() { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(10, 0, 0, 20) };
        layoutBehavior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        layoutBehavior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

        chkInteractive = new CheckBox() { Text = "Interactive Mode (Mouse Moves Stars)", AutoSize = true, Checked = true, FlatStyle = FlatStyle.System };
        layoutBehavior.Controls.Add(chkInteractive, 0, 0);
        layoutBehavior.SetColumnSpan(chkInteractive, 2);

        // Speed Controls
        chkEnableSpeedIncrease = new CheckBox() { Text = "Increase Speed Over Time", AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(3, 10, 3, 3) };
        chkEnableSpeedIncrease.CheckedChanged += (s, e) => ToggleSpeedControls();
        layoutBehavior.Controls.Add(chkEnableSpeedIncrease, 0, 1);
        layoutBehavior.SetColumnSpan(chkEnableSpeedIncrease, 2);

        layoutBehavior.Controls.Add(new Label() { Text = "Speed Increase Interval (Minutes):", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Padding = new Padding(0, 10, 0, 0) }, 0, 2);
        numSpeedInterval = new NumericUpDown() { Minimum = 1, Maximum = 1440, Value = 1, Width = 100, Margin = new Padding(3, 10, 3, 3) };
        layoutBehavior.Controls.Add(numSpeedInterval, 1, 2);

        layoutBehavior.Controls.Add(new Label() { Text = "Speed Increment Amount:", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Padding = new Padding(0, 10, 0, 0) }, 0, 3);
        numSpeedIncrement = new NumericUpDown() { Minimum = 0, Maximum = 1, DecimalPlaces = 4, Increment = 0.0001M, Value = 0.0001M, Width = 100, Margin = new Padding(3, 10, 3, 3) };
        layoutBehavior.Controls.Add(numSpeedIncrement, 1, 3);

        // Nebula Controls
        chkEnableNebula = new CheckBox() { Text = "Enable Nebula Mode (Random Colored Areas)", AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(3, 20, 3, 3) };
        chkEnableNebula.CheckedChanged += (s, e) => ToggleSpeedControls();
        layoutBehavior.Controls.Add(chkEnableNebula, 0, 4);
        layoutBehavior.SetColumnSpan(chkEnableNebula, 2);

        layoutBehavior.Controls.Add(new Label() { Text = "Nebula Interval (Hours:Minutes):", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Padding = new Padding(0, 10, 0, 0) }, 0, 5);
        nebulaIntervalPanel = new FlowLayoutPanel() { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(3, 10, 3, 3) };
        numNebulaHour = new NumericUpDown() { Minimum = 0, Maximum = 168, Value = 0, Width = 60 };
        var lblColon = new Label() { Text = ":", AutoSize = false, Width = 15, Height = 23, TextAlign = ContentAlignment.MiddleCenter, Margin = new Padding(0) };
        numNebulaMinute = new NumericUpDown() { Minimum = 0, Maximum = 59, Value = 30, Width = 60 };
        nebulaIntervalPanel.Controls.Add(numNebulaHour);
        nebulaIntervalPanel.Controls.Add(lblColon);
        nebulaIntervalPanel.Controls.Add(numNebulaMinute);
        layoutBehavior.Controls.Add(nebulaIntervalPanel, 1, 5);

        layoutBehavior.Controls.Add(new Label() { Text = "Nebula Duration (Minutes):", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Padding = new Padding(0, 10, 0, 0) }, 0, 6);
        numNebulaDuration = new NumericUpDown() { Minimum = 1, Maximum = 720, Value = 10, Width = 100, Margin = new Padding(3, 10, 3, 3) };
        layoutBehavior.Controls.Add(numNebulaDuration, 1, 6);
        
        chkResetOnToggle = new CheckBox() { Text = "Reset Speeds When Toggling Modes (R)", AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(3, 10, 3, 3), Checked = true };
        layoutBehavior.Controls.Add(chkResetOnToggle, 0, 7);
        layoutBehavior.SetColumnSpan(chkResetOnToggle, 2);


        // --- Buttons ---
        var panelButtons = new FlowLayoutPanel() { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };
        // Removing padding/margin from FlowLayoutPanel to make it flush
        panelButtons.Margin = new Padding(0);
        
        btnCancel = new Button() { Text = "Cancel", FlatStyle = FlatStyle.System, Height = 30, Width = 80 };
        btnSave = new Button() { Text = "Save", FlatStyle = FlatStyle.System, Height = 30, Width = 80 };
        btnSave.Click += (s, e) => { SaveSettings(); this.Close(); };
        btnCancel.Click += (s, e) => { this.Close(); };
        
        // Add spaces between buttons
        btnCancel.Margin = new Padding(10, 0, 0, 0);
        
        panelButtons.Controls.Add(btnCancel);
        panelButtons.Controls.Add(btnSave);

        // Add to Main Layout
        mainLayout.Controls.Add(lblHeader1, 0, 0);
        mainLayout.Controls.Add(layoutAppearance, 0, 1);
        mainLayout.Controls.Add(lblHeader2, 0, 2);
        mainLayout.Controls.Add(layoutBehavior, 0, 3);
        
        panelButtons.Dock = DockStyle.Bottom;
        mainLayout.Dock = DockStyle.Fill;
        this.Controls.Add(panelButtons);
        this.Controls.Add(mainLayout);
        this.AcceptButton = btnSave;
        this.CancelButton = btnCancel;
    }

    private void ToggleSpeedControls()
    {
        numSpeedInterval.Enabled = chkEnableSpeedIncrease.Checked;
        numSpeedIncrement.Enabled = chkEnableSpeedIncrease.Checked;
        nebulaIntervalPanel.Enabled = chkEnableNebula.Checked;
        numNebulaDuration.Enabled = chkEnableNebula.Checked;
    }

    private void LoadSettings()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\StarfieldScreensaver");
        if (key != null)
        {
            numStarCount.Value = Convert.ToDecimal(key.GetValue("StarCountDivisor", 8));
            numStarSize.Value = Convert.ToDecimal(key.GetValue("StarSize", 3));
            chkInteractive.Checked = Convert.ToBoolean(key.GetValue("Interactive", "True"));
            
            chkEnableSpeedIncrease.Checked = Convert.ToBoolean(key.GetValue("EnableSpeedIncrease", "False"));
            double spdSec = Convert.ToDouble(key.GetValue("SpeedIncreaseInterval", 60));
            numSpeedInterval.Value = Convert.ToDecimal(spdSec / 60.0);
            
            string speedInc = key.GetValue("SpeedIncrement", "0.0001").ToString();
            decimal incVal;
            if(decimal.TryParse(speedInc, out incVal)) numSpeedIncrement.Value = incVal;

            chkEnableNebula.Checked = Convert.ToBoolean(key.GetValue("EnableNebula", "False"));
            double nebulaIntervalSec = Convert.ToDouble(key.GetValue("NebulaInterval", 30));
            int hours = (int)(nebulaIntervalSec / 3600);
            int minutes = (int)((nebulaIntervalSec % 3600) / 60);
            numNebulaHour.Value = hours;
            numNebulaMinute.Value = minutes;
            numNebulaDuration.Value = Convert.ToDecimal(key.GetValue("NebulaDurationMinutes", 10));
            chkResetOnToggle.Checked = Convert.ToBoolean(key.GetValue("ResetOnToggle", "True"));
            
            key.Close();
        }
        ToggleSpeedControls();
    }

    private void SaveSettings()
    {
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\StarfieldScreensaver");
        key.SetValue("StarCountDivisor", numStarCount.Value);
        key.SetValue("StarSize", numStarSize.Value);
        key.SetValue("Interactive", chkInteractive.Checked);
        key.SetValue("EnableSpeedIncrease", chkEnableSpeedIncrease.Checked);
        key.SetValue("SpeedIncreaseInterval", (int)numSpeedInterval.Value * 60);
        key.SetValue("SpeedIncrement", numSpeedIncrement.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        
        key.SetValue("EnableNebula", chkEnableNebula.Checked);
        int nebulaIntervalSeconds = (int)numNebulaHour.Value * 3600 + (int)numNebulaMinute.Value * 60;
        key.SetValue("NebulaInterval", nebulaIntervalSeconds);
        key.SetValue("NebulaDurationMinutes", (int)numNebulaDuration.Value);
        key.SetValue("ResetOnToggle", chkResetOnToggle.Checked);
        
        key.Close();
    }
}

public class StarfieldScreenSaver : Form
{
    private WebBrowser webBrowser;
    private Point mouseLocation;
    private bool interactiveMode = true;

    // Embedded HTML Content Template
    private const string HtmlTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Starfield Screensaver</title>
    <style>
        body { 
          width: 100%; 
          height: 100vh; 
          background-color: #000; 
          background-image: radial-gradient(circle at top right, rgba(121, 68, 154, 0.13),       transparent), 
            radial-gradient(circle at 20% 80%, rgba(41, 196, 255, 0.13), transparent);
          margin: 0;
          overflow: hidden;
        } 
        canvas { 
          position: fixed; 
          width: 100%; 
          height: 100%; 
          top: 0; left: 0;
        } 
        #webgl-canvas {
            z-index: 10;
            display: none;
            opacity: 0;
            transition: opacity 1s ease-in-out;
        }
        #starfield-canvas {
            z-index: 1;
            transition: opacity 1s ease-in-out;
        }
    </style>
</head>
<body>
    <canvas id=""starfield-canvas""></canvas>
    <canvas id=""webgl-canvas""></canvas>
    <script>
        // INJECT_SETTINGS_HERE
        var STAR_COLOR = '#fff'; 
        var STAR_MIN_SCALE = 0.2; 
        var OVERFLOW_THRESHOLD = 50; 
        var STAR_COUNT = ( window.innerWidth + window.innerHeight ) / STAR_COUNT_DIVISOR; 

        var canvas = document.getElementById( 'starfield-canvas' ), 
              context = canvas.getContext( '2d' ); 
        var glCanvas = document.getElementById( 'webgl-canvas' );

        var scale = 1, // device pixel ratio 
            width, 
            height; 
            
        var stars = []; 

        var pointerX, 
            pointerY; 
        var referenceX = null,
            referenceY = null;

        var velocity = { x: 0, y: 0, tx: 0, ty: 0, z: 0.0002 }; 

        var touchInput = false; 
        var lastSpeedIncreaseTime = Date.now();
        var lastNebulaCheckTime = Date.now();
        var isNebulaActive = false;
        var nebulaStartTime = 0;
        var NEBULA_DURATION = (typeof NEBULA_DURATION !== 'undefined') ? NEBULA_DURATION : 600000;
        var nebulaTime = 0;
        var lastFrameTime = Date.now();

        var glContext = null;
        var glProgram = null;

        // --- WebGL Shader Source ---
        var vsSource = 'attribute vec2 position; void main() { gl_Position = vec4(position, 0.0, 1.0); }';
        var fsSource = 'precision mediump float;' +
                       'uniform vec2 iResolution;' +
                       'uniform float iTime;' +
                       'uniform vec2 iMouse;' +
                       '\n#define NUM_LAYERS 8.0\n' +
                       'mat2 Rot(float a) {' +
                       '    float s = sin(a);' +
                       '    float c = cos(a);' + 
                       '    return mat2(c, -s, s, c);' +
                       '}' +
                       'float Star(vec2 uv, float flare) {' +
                       '    float d = length(uv);' +
                       '    float m = 0.05 / d;' +
                       '    float rays = max(0.0, 1.0 - abs(uv.x * uv.y * 1000.0));' +
                       '    m += rays * flare;' +
                       '    uv *= Rot(3.1415 * 0.25);' +
                       '    rays = max(0.0, 1.0 - abs(uv.x * uv.y * 1000.0));' +
                       '    m += rays * 0.3 * flare;' +
                       '    m *= smoothstep(1.0, 0.2, d);' +
                       '    return m;' +
                       '}' +
                       'float Hash21(vec2 p) {' +
                       '    p = fract(p * vec2(123.34, 456.21));' +
                       '    p += dot(p, p + 45.32);' +
                       '    return fract(p.x * p.y);' +
                       '}' +
                       'vec3 StarLayer(vec2 uv) {' +
                       '    vec3 col = vec3(0);' +
                       '    vec2 gv = fract(uv) - 0.5;' +
                       '    vec2 id = floor(uv);' +
                       '    for(int y = -1; y <= 1; y++) {' +
                       '        for(int x = -1; x <= 1; x++) {' +
                       '            vec2 offs = vec2(x, y);' +
                       '            float n = Hash21(id + offs);' +
                       '            float size = fract(n * 345.32);' +
                       '            float star = Star(gv - offs - vec2(n, fract(n * 34.0)) + 0.5, smoothstep(0.9, 1.0, size) * 0.6);' +
                       '            vec3 color = sin(vec3(0.2, 0.3, 0.9) * fract(n * 2345.2) * 123.2) * 0.5 + 0.5;' +
                       '            color = color * vec3(1.0, 0.5, 1.0 + size);' +
                       '            col += star * size * color;' +
                       '        }' +
                       '    }' +
                       '    return col;' +
                       '}' +
                       'void main() {' +
                       '    vec2 uv = (gl_FragCoord.xy - 0.5 * iResolution.xy) / iResolution.y;' +
                       '    uv += iMouse;' +
                       '    float t = iTime;' +
                       '    uv.x += sin(t) * 0.2;' +
                       '    vec3 col = vec3(0);' +
                       '    for(float i = 0.0; i < 1.0; i += 1.0 / NUM_LAYERS) {' +
                       '        float depth = fract(i + t);' +
                       '        float scale = mix(20.0, 0.5, depth);' +
                       '        float fade = depth * smoothstep(1.0, 0.9, depth);' +
                       '        col += StarLayer(uv * scale + i * 453.2) * fade;' +
                       '    }' +
                       '    gl_FragColor = vec4(col, 1.0);' +
                       '}';

        generate(); 
        resize(); 
        step(); 

        window.onresize = resize; 
        window.onmousemove = onMouseMove; 
        window.ontouchmove = onTouchMove; 
        window.ontouchend = onMouseLeave; 
        document.onmouseleave = onMouseLeave; 
        
        // Wheel support for speed
        window.onwheel = onScroll;
        // IE11 fallback
        window.addEventListener('wheel', onScroll);

        // Key support for Nebula toggle - Handled by C# ProcessCmdKey
        // window.addEventListener('keydown', function(e) { ... });
        
        function onScroll(event) {
            var delta = event.deltaY || -event.detail || event.wheelDelta;
            if(delta < 0) {
                velocity.z += 0.0001;
            } else {
                velocity.z -= 0.0001;
            }
            // Clamp velocity
            if(velocity.z < 0.0001) velocity.z = 0.0001;
            if(velocity.z > 0.01) velocity.z = 0.01;
        }

        function toggleNebula() {
            isNebulaActive = !isNebulaActive;
            if(isNebulaActive) {
                nebulaStartTime = Date.now();
                lastFrameTime = Date.now();
                if (typeof RESET_ON_TOGGLE === 'undefined' || RESET_ON_TOGGLE) {
                    velocity.x = 0; velocity.y = 0; velocity.tx = 0; velocity.ty = 0; velocity.z = 0.0002;
                    referenceX = null; referenceY = null;
                    nebulaTime = 0;
                } else {
                    nebulaTime = 0;
                }
                initWebGL();
                glCanvas.style.display = 'block';
                // Trigger reflow
                void glCanvas.offsetWidth; 
                glCanvas.style.opacity = 1;
                canvas.style.opacity = 0;
            } else {
                if (typeof RESET_ON_TOGGLE === 'undefined' || RESET_ON_TOGGLE) {
                    velocity.x = 0; velocity.y = 0; velocity.tx = 0; velocity.ty = 0; velocity.z = 0.0002;
                    referenceX = null; referenceY = null;
                    stars = []; generate(); stars.forEach( placeStar );
                }
                glCanvas.style.opacity = 0;
                canvas.style.opacity = 1;
                setTimeout(function() {
                     if(!isNebulaActive) glCanvas.style.display = 'none';
                }, 1000);
            }
        }

        function initWebGL() {
            if(glContext) return;
            try {
                glContext = glCanvas.getContext('webgl') || glCanvas.getContext('experimental-webgl');
                if(!glContext) return;

                var vs = glContext.createShader(glContext.VERTEX_SHADER);
                glContext.shaderSource(vs, vsSource);
                glContext.compileShader(vs);
                if (!glContext.getShaderParameter(vs, glContext.COMPILE_STATUS)) return;

                var fs = glContext.createShader(glContext.FRAGMENT_SHADER);
                glContext.shaderSource(fs, fsSource);
                glContext.compileShader(fs);
                if (!glContext.getShaderParameter(fs, glContext.COMPILE_STATUS)) return;

                glProgram = glContext.createProgram();
                glContext.attachShader(glProgram, vs);
                glContext.attachShader(glProgram, fs);
                glContext.linkProgram(glProgram);
                if (!glContext.getProgramParameter(glProgram, glContext.LINK_STATUS)) return;
                
                glContext.useProgram(glProgram);

                var buffer = glContext.createBuffer();
                glContext.bindBuffer(glContext.ARRAY_BUFFER, buffer);
                glContext.bufferData(
                    glContext.ARRAY_BUFFER, 
                    new Float32Array([-1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0]), 
                    glContext.STATIC_DRAW
                );

                var positionLocation = glContext.getAttribLocation(glProgram, 'position');
                glContext.enableVertexAttribArray(positionLocation);
                glContext.vertexAttribPointer(positionLocation, 2, glContext.FLOAT, false, 0, 0);
                
                resizeWebGL();
            } catch(e) { }
        }

        function resizeWebGL() {
            if(!glContext) return;
            glCanvas.width = window.innerWidth * (window.devicePixelRatio || 1);
            glCanvas.height = window.innerHeight * (window.devicePixelRatio || 1);
            glContext.viewport(0, 0, glCanvas.width, glCanvas.height);
        }

        function renderWebGL() {
            if(!glContext || !isNebulaActive) return;

            // Time Calc
            var now = Date.now();
            var dt = (now - lastFrameTime) / 1000.0;
            // Map velocity.z (0.0005 to 0.01) to speed multiplier.
            // 0.0005 * 800 = 0.4 (original speed)
            var elapsed = (now - nebulaStartTime) / 1000.0;
            var ramp = elapsed < 3.0 ? (elapsed / 3.0) : 1.0;
            nebulaTime += dt * (velocity.z * 800.0) * ramp;

            var uResolution = glContext.getUniformLocation(glProgram, 'iResolution');
            var uTime = glContext.getUniformLocation(glProgram, 'iTime');
            var uMouse = glContext.getUniformLocation(glProgram, 'iMouse');
            
            glContext.uniform2f(uResolution, glCanvas.width, glCanvas.height);
            glContext.uniform1f(uTime, nebulaTime);
            
            // Mouse Interaction
            var mx = 0, my = 0;
            if(typeof pointerX === 'number') {
                if (referenceX === null) {
                    referenceX = pointerX;
                    referenceY = pointerY;
                }
                
                // Calculate delta from reference (Relative Parallax)
                // This ensures it starts centered (0,0) and moves from there
                var dx = pointerX - referenceX;
                var dy = pointerY - referenceY;
                
                mx = (dx / width) * 2.0; 
                my = (dy / height) * 2.0; 
            }
            // Invert X to make background move opposite to look direction (parallax effect)
            glContext.uniform2f(uMouse, -mx, my);

            glContext.drawArrays(glContext.TRIANGLES, 0, 6);
        }

        function generate() { 
           for( var i = 0; i < STAR_COUNT; i++ ) { 
            stars.push({ 
              x: 0, 
              y: 0, 
              z: STAR_MIN_SCALE + Math.random() * ( 1 - STAR_MIN_SCALE ) 
            }); 
           } 
        } 

        function placeStar( star ) { 
          star.x = Math.random() * width; 
          star.y = Math.random() * height; 
        } 

        function recycleStar( star ) { 
          var direction = 'z'; 
          var vx = Math.abs( velocity.x ), 
                vy = Math.abs( velocity.y ); 

          if( vx > 1 || vy > 1 ) { 
            var axis; 
            if( vx > vy ) { 
              axis = Math.random() < vx / ( vx + vy ) ? 'h' : 'v'; 
            } 
            else { 
              axis = Math.random() < vy / ( vx + vy ) ? 'v' : 'h'; 
            } 

            if( axis === 'h' ) { 
              direction = velocity.x > 0 ? 'l' : 'r'; 
            } 
            else { 
              direction = velocity.y > 0 ? 't' : 'b'; 
            } 
          } 
          
          star.z = STAR_MIN_SCALE + Math.random() * ( 1 - STAR_MIN_SCALE ); 

          if( direction === 'z' ) { 
            star.z = 0.1; 
            star.x = Math.random() * width; 
            star.y = Math.random() * height; 
          } 
          else if( direction === 'l' ) { 
            star.x = -OVERFLOW_THRESHOLD; 
            star.y = height * Math.random(); 
          } 
          else if( direction === 'r' ) { 
            star.x = width + OVERFLOW_THRESHOLD; 
            star.y = height * Math.random(); 
          } 
          else if( direction === 't' ) { 
            star.x = width * Math.random(); 
            star.y = -OVERFLOW_THRESHOLD; 
          } 
          else if( direction === 'b' ) { 
            star.x = width * Math.random(); 
            star.y = height + OVERFLOW_THRESHOLD; 
          } 
        } 

        function resize() { 
          scale = window.devicePixelRatio || 1; 
          width = window.innerWidth * scale; 
          height = window.innerHeight * scale; 
          canvas.width = width; 
          canvas.height = height; 
          stars.forEach( placeStar ); 
          resizeWebGL();
        } 

        function step() { 
            
            if (isNebulaActive) {
                renderWebGL();
            } else {
                context.clearRect( 0, 0, width, height ); 
                update(); 
                render(); 
            }
            
            // Handle Nebula Auto-Toggle (Activate)
            if(ENABLE_NEBULA && !isNebulaActive && (Date.now() - lastNebulaCheckTime > NEBULA_INTERVAL)) {
                toggleNebula();
                lastNebulaCheckTime = Date.now();
            }
            
            // Handle Nebula Duration (Deactivate after 10 mins)
            if(isNebulaActive && (Date.now() - nebulaStartTime > NEBULA_DURATION)) {
                toggleNebula();
                // Reset check time so it doesn't immediately reactivate
                lastNebulaCheckTime = Date.now();
            }
            
            lastFrameTime = Date.now();
            requestAnimationFrame( step ); 
        } 

        function update() { 
          // Dynamic Speed Increase
          if (ENABLE_SPEED_INCREASE && (Date.now() - lastSpeedIncreaseTime > SPEED_INCREASE_INTERVAL)) {
              velocity.z += SPEED_INCREMENT;
              if(velocity.z > 0.1) velocity.z = 0.1; // Safety cap
              lastSpeedIncreaseTime = Date.now();
          }

          velocity.tx *= 0.96; 
          velocity.ty *= 0.96; 
          velocity.x += ( velocity.tx - velocity.x ) * 0.8; 
          velocity.y += ( velocity.ty - velocity.y ) * 0.8; 

          stars.forEach( function( star ) { 
            star.x += velocity.x * star.z; 
            star.y += velocity.y * star.z; 
            star.x += ( star.x - width/2 ) * velocity.z * star.z; 
            star.y += ( star.y - height/2 ) * velocity.z * star.z; 
            star.z += velocity.z; 
          
            if( star.x < -OVERFLOW_THRESHOLD || star.x > width + OVERFLOW_THRESHOLD || star.y < -OVERFLOW_THRESHOLD || star.y > height + OVERFLOW_THRESHOLD ) { 
              recycleStar( star ); 
            } 
          } ); 
        } 

        function render() { 
          stars.forEach( function( star ) { 
            context.beginPath(); 
            context.lineCap = 'round'; 
            context.lineWidth = STAR_SIZE * star.z * scale; 
            context.globalAlpha = 0.5 + 0.5*Math.random(); 
            context.strokeStyle = STAR_COLOR; 
            context.beginPath(); 
            context.moveTo( star.x, star.y ); 

            var tailX = velocity.x * 2, 
                tailY = velocity.y * 2; 
            if( Math.abs( tailX ) < 0.1 ) tailX = 0.5; 
            if( Math.abs( tailY ) < 0.1 ) tailY = 0.5; 

            context.lineTo( star.x + tailX, star.y + tailY ); 
            context.stroke(); 
          } ); 
        } 

        function movePointer( x, y ) { 
          if( typeof pointerX === 'number' && typeof pointerY === 'number' ) { 
            var ox = x - pointerX, 
                oy = y - pointerY; 
            velocity.tx = velocity.tx + ( ox / 8*scale ) * ( touchInput ? 1 : -1 ); 
            velocity.ty = velocity.ty + ( oy / 8*scale ) * ( touchInput ? 1 : -1 ); 
          } 
          pointerX = x; 
          pointerY = y; 
        } 

        function onMouseMove( event ) { 
          touchInput = false; 
          movePointer( event.clientX, event.clientY ); 
        } 

        function onTouchMove( event ) { 
          touchInput = true; 
          movePointer( event.touches[0].clientX, event.touches[0].clientY, true ); 
          event.preventDefault(); 
        } 

        function onMouseLeave() { 
          pointerX = null; 
          pointerY = null; 
          referenceX = null;
          referenceY = null;
        }
    </script>
</body>
</html>";

    public StarfieldScreenSaver()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.ShowInTaskbar = false;
        this.TopMost = true;

        webBrowser = new WebBrowser();
        webBrowser.Dock = DockStyle.Fill;
        webBrowser.ScrollBarsEnabled = false;
        webBrowser.ScriptErrorsSuppressed = true;
        webBrowser.IsWebBrowserContextMenuEnabled = false;

        // Load Settings
        string starCountDivisor = "8";
        string starSize = "3";
        bool enableSpeedIncrease = false;
        double speedInterval = 60; // seconds
        double speedIncrement = 0.0001;
        bool enableNebula = false;
        double nebulaInterval = 30; // seconds
        double nebulaDurationMinutes = 10;
        bool resetOnToggle = true;

        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\StarfieldScreensaver");
        if (key != null)
        {
            starCountDivisor = key.GetValue("StarCountDivisor", "8").ToString();
            starSize = key.GetValue("StarSize", "3").ToString();
            interactiveMode = Convert.ToBoolean(key.GetValue("Interactive", "True"));
            enableSpeedIncrease = Convert.ToBoolean(key.GetValue("EnableSpeedIncrease", "False"));
            speedInterval = Convert.ToDouble(key.GetValue("SpeedIncreaseInterval", 60));
            
            string speedIncStr = key.GetValue("SpeedIncrement", "0.0001").ToString();
            double.TryParse(speedIncStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedIncrement);

            enableNebula = Convert.ToBoolean(key.GetValue("EnableNebula", "False"));
            nebulaInterval = Convert.ToDouble(key.GetValue("NebulaInterval", 30));
            nebulaDurationMinutes = Convert.ToDouble(key.GetValue("NebulaDurationMinutes", 10));
            resetOnToggle = Convert.ToBoolean(key.GetValue("ResetOnToggle", "True"));

            key.Close();
        }

        string settingsScript = String.Format(
            "var STAR_COUNT_DIVISOR = {0}; var STAR_SIZE = {1}; var ENABLE_SPEED_INCREASE = {2}; var SPEED_INCREASE_INTERVAL = {3}; var SPEED_INCREMENT = {4}; var ENABLE_NEBULA = {5}; var NEBULA_INTERVAL = {6}; var NEBULA_DURATION = {7}; var RESET_ON_TOGGLE = {8};", 
            starCountDivisor, 
            starSize, 
            enableSpeedIncrease.ToString().ToLower(), 
            speedInterval * 1000, // Convert to ms for JS
            speedIncrement.ToString(System.Globalization.CultureInfo.InvariantCulture),
            enableNebula.ToString().ToLower(),
            nebulaInterval * 1000, // Convert to ms for JS
            nebulaDurationMinutes * 60 * 1000, // Convert to ms for JS
            resetOnToggle.ToString().ToLower());

        string htmlContent = HtmlTemplate.Replace("// INJECT_SETTINGS_HERE", settingsScript);
        
        // Write HTML to temp file
        string tempPath = Path.Combine(Path.GetTempPath(), "starfield_scr_" + Guid.NewGuid().ToString() + ".html");
        File.WriteAllText(tempPath, htmlContent);
        
        webBrowser.Navigate(new Uri(tempPath));

        this.Controls.Add(webBrowser);
        this.Load += StarfieldScreenSaver_Load;
        this.FormClosed += (s, e) => {
            try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
        };
        
        Timer t = new Timer();
        t.Interval = 100;
        t.Tick += T_Tick;
        t.Start();
        
        Cursor.Hide();
    }

    private void StarfieldScreenSaver_Load(object sender, EventArgs e)
    {
        mouseLocation = Cursor.Position;
    }

    private void T_Tick(object sender, EventArgs e)
    {
        // Only exit on significant mouse move if NOT in interactive mode
        if (!interactiveMode)
        {
            if (Math.Abs(Cursor.Position.X - mouseLocation.X) > 10 ||
                Math.Abs(Cursor.Position.Y - mouseLocation.Y) > 10)
            {
                Application.Exit();
            }
        }
        
        // ALWAYS exit on mouse click
        if (Control.MouseButtons != MouseButtons.None)
        {
            Application.Exit();
        }
    }
    
    // Override ProcessCmdKey to catch keystrokes even if WebBrowser has focus
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Allow 'R' key to toggle Nebula mode
        if ((keyData & Keys.KeyCode) == Keys.R)
        {
            try {
                if (webBrowser != null && webBrowser.Document != null)
                {
                    webBrowser.Document.InvokeScript("toggleNebula");
                }
            } catch {}
            return true;
        }

        Application.Exit();
        return true;
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    private static void SetBrowserFeatureControl()
    {
        // FeatureControl settings are per-process
        var fileName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        // FEATURE_BROWSER_EMULATION
        // 11001 (0x2AF9) = IE11 Edge Mode
        using (var key = Registry.CurrentUser.CreateSubKey(
            @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
        {
            key.SetValue(fileName, 11001, RegistryValueKind.DWord);
        }

        // FEATURE_GPU_RENDERING
        // 1 = Enable
        using (var key = Registry.CurrentUser.CreateSubKey(
            @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING"))
        {
            key.SetValue(fileName, 1, RegistryValueKind.DWord);
        }
    }

    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try { SetProcessDPIAware(); } catch { }
        try { SetBrowserFeatureControl(); } catch { }

        if (args.Length > 0)
        {
            string firstArg = args[0].ToLower().Trim();
            string mode = firstArg.Length >= 2 ? firstArg.Substring(0, 2) : firstArg;
            
            if (mode == "/c")
            {
                Application.Run(new SettingsForm());
            }
            else if (mode == "/p")
            {
                // Preview mode
            }
            else if (mode == "/s")
            {
                Application.Run(new StarfieldScreenSaver());
            }
            else
            {
                Application.Run(new StarfieldScreenSaver());
            }
        }
        else
        {
            Application.Run(new StarfieldScreenSaver());
        }
    }
}
