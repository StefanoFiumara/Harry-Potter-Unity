// Copyright (c) 2011 Bob Berkebile (pixelplacment)
// Please direct any bugs/comments/suggestions to http://pixelplacement.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/*
TERMS OF USE - EASING EQUATIONS
Open source under the BSD License.
Copyright (c)2001 Robert Penner
All rights reserved.
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#endregion

/// <summary>
/// <para>Version: 2.0.46</para>	 
/// <para>Author: Bob Berkebile (http://pixelplacement.com)</para>
/// <para>Support: http://itween.pixelplacement.com</para>
/// </summary>
public class iTween : MonoBehaviour
{

    #region Variables

    //repository of all living iTweens:
    public static List<Hashtable> Tweens = new List<Hashtable>();

    //camera fade object:
    private static GameObject _cameraFade;

    //status members (made public for visual troubleshooting in the inspector):
    public string Id, Type, Method;
    public iTween.EaseType easeType;
    public float Time, Delay;
    public LoopType loopType;
    public bool IsRunning, IsPaused;
    /* GFX47 MOD START */
    public string Name;
    /* GFX47 MOD END */

    //private members:
    private float _runningTime, _percentage;
    private float _delayStarted; //probably not neccesary that this be protected but it shuts Unity's compiler up about this being "never used"
    private bool _kinematic, _isLocal, _loop, _reverse, _wasPaused, _physics;
    private Hashtable _tweenArguments;
    private Space _space;
    private delegate float EasingFunction(float start, float end, float value);
    private delegate void ApplyTween();
    private EasingFunction _ease;
    private ApplyTween _apply;
    private AudioSource _audioSource;
    private Vector3[] _vector3S;
    private Vector2[] _vector2S;
    private Color[,] _colors;
    private float[] _floats;
    private Rect[] _rects;
    private CrSpline _path;
    private Vector3 _preUpdate;
    private Vector3 _postUpdate;
    private NamedValueColor _namedcolorvalue;

    private float _lastRealTime; // Added by PressPlay
    private bool _useRealTime; // Added by PressPlay

    private Transform _thisTransform;


    /// <summary>
    /// The type of easing to use based on Robert Penner's open source easing equations (http://www.robertpenner.com/easing_terms_of_use.html).
    /// </summary>
    public enum EaseType
    {
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        Linear,
        Spring,
        /* GFX47 MOD START */
        //bounce,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        /* GFX47 MOD END */
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        /* GFX47 MOD START */
        //elastic,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        /* GFX47 MOD END */
        Punch
    }

    /// <summary>
    /// The type of loop (if any) to use.  
    /// </summary>
    public enum LoopType
    {
        /// <summary>
        /// Do not loop.
        /// </summary>
        None,
        /// <summary>
        /// Rewind and replay.
        /// </summary>
        Loop,
        /// <summary>
        /// Ping pong the animation back and forth.
        /// </summary>
        PingPong
    }

    /// <summary>
    /// Many shaders use more than one color. Use can have iTween's Color methods operate on them by name.   
    /// </summary>
    public enum NamedValueColor
    {
        /// <summary>
        /// The main color of a material. Used by default and not required for Color methods to work in iTween.
        /// </summary>
        Color,
        /// <summary>
        /// The specular color of a material (used in specular/glossy/vertexlit shaders).
        /// </summary>
        SpecColor,
        /// <summary>
        /// The emissive color of a material (used in vertexlit shaders).
        /// </summary>
        Emission,
        /// <summary>
        /// The reflection color of the material (used in reflective shaders).
        /// </summary>
        ReflectColor
    }

    #endregion

    #region Defaults

    /// <summary>
    /// A collection of baseline presets that iTween needs and utilizes if certain parameters are not provided. 
    /// </summary>
    public static class Defaults
    {
        //general defaults:
        public static float Time = 1f;
        public static float Delay = 0f;
        public static NamedValueColor NamedColorValue = NamedValueColor.Color;
        public static LoopType LoopType = LoopType.None;
        public static EaseType EaseType = iTween.EaseType.EaseOutExpo;
        public static float LookSpeed = 3f;
        public static bool IsLocal = false;
        public static Space Space = Space.Self;
        public static bool OrientToPath = false;
        public static Color Color = Color.white;
        //update defaults:
        public static float UpdateTimePercentage = .05f;
        public static float UpdateTime = 1f * UpdateTimePercentage;
        //cameraFade defaults:
        public static int cameraFadeDepth = 999999;
        //path look ahead amount:
        public static float LookAhead = .05f;
        public static bool UseRealTime = false; // Added by PressPlay
                                                //look direction:
        public static Vector3 Up = Vector3.up;
    }

    #endregion

    #region #1 Static Registers

    /// <summary>
    /// Sets up a GameObject to avoid hiccups when an initial iTween is added. It's advisable to run this on every object you intend to run iTween on in its Start or Awake.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target to be initialized for iTween.
    /// </param>
    public static void Init(GameObject target)
    {
        MoveBy(target, Vector3.zero, 0);
    }

    /// <summary>
    /// Instantly changes the amount(transparency) of a camera fade and then returns it back over time with MINIMUM customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how transparent the Texture2D that the camera fade uses is.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void CameraFadeFrom(float amount, float time)
    {
        if (_cameraFade)
        {
            CameraFadeFrom(Hash("amount", amount, "time", time));
        }
        else
        {
            Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
        }
    }

    /// <summary>
    /// Instantly changes the amount(transparency) of a camera fade and then returns it back over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how transparent the Texture2D that the camera fade uses is.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void CameraFadeFrom(Hashtable args)
    {
        //establish iTween:
        if (_cameraFade)
        {
            ColorFrom(_cameraFade, args);
        }
        else
        {
            Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
        }
    }

    /// <summary>
    /// Changes the amount(transparency) of a camera fade over time with MINIMUM customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how transparent the Texture2D that the camera fade uses is.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void CameraFadeTo(float amount, float time)
    {
        if (_cameraFade)
        {
            CameraFadeTo(Hash("amount", amount, "time", time));
        }
        else
        {
            Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
        }
    }

    /// <summary>
    /// Changes the amount(transparency) of a camera fade over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how transparent the Texture2D that the camera fade uses is.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void CameraFadeTo(Hashtable args)
    {
        /*
		CameraFadeAdd(Defaults.cameraFadeDepth);
		
		//rescale cameraFade just in case screen size has changed to ensure it takes up the full screen:
		cameraFade.guiTexture.pixelInset=new Rect(0,0,Screen.width,Screen.height);
		*/

        if (_cameraFade)
        {
            //establish iTween:
            ColorTo(_cameraFade, args);
        }
        else
        {
            Debug.LogError("iTween Error: You must first add a camera fade object with CameraFadeAdd() before atttempting to use camera fading.");
        }
    }

    /// <summary>
    /// Returns a value to an 'oncallback' method interpolated between the supplied 'from' and 'to' values for application as desired.  Requires an 'onupdate' callback that accepts the same type as the supplied 'from' and 'to' properties.
    /// </summary>
    /// <param name="from">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> or <see cref="Vector3"/> or <see cref="Vector2"/> or <see cref="Color"/> or <see cref="Rect"/> for the starting value.
    /// </param> 
    /// <param name="to">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> or <see cref="Vector3"/> or <see cref="Vector2"/> or <see cref="Color"/> or <see cref="Rect"/> for the ending value.
    /// </param> 
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed (only works with Vector2, Vector3, and Floats)
    /// </param>	
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ValueTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        if (!args.Contains("onupdate") || !args.Contains("from") || !args.Contains("to"))
        {
            Debug.LogError("iTween Error: ValueTo() requires an 'onupdate' callback function and a 'from' and 'to' property.  The supplied 'onupdate' callback must accept a single argument that is the same type as the supplied 'from' and 'to' properties!");
            return;
        }
        else
        {
            //establish iTween:
            args["type"] = "value";

            if (args["from"].GetType() == typeof(Vector2))
            {
                args["method"] = "vector2";
            }
            else if (args["from"].GetType() == typeof(Vector3))
            {
                args["method"] = "vector3";
            }
            else if (args["from"].GetType() == typeof(Rect))
            {
                args["method"] = "rect";
            }
            else if (args["from"].GetType() == typeof(Single))
            {
                args["method"] = "float";
            }
            else if (args["from"].GetType() == typeof(Color))
            {
                args["method"] = "color";
            }
            else
            {
                Debug.LogError("iTween Error: ValueTo() only works with interpolating Vector3s, Vector2s, floats, ints, Rects and Colors!");
                return;
            }

            //set a default easeType of linear if none is supplied since eased color interpolation is nearly unrecognizable:
            if (!args.Contains("easetype"))
            {
                args.Add("easetype", EaseType.Linear);
            }

            Launch(target, args);
        }
    }

    /// <summary>
    /// Changes a GameObject's alpha value instantly then returns it to the provided alpha over time with MINIMUM customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation. Identical to using ColorFrom and using the "a" parameter. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void FadeFrom(GameObject target, float alpha, float time)
    {
        FadeFrom(target, Hash("alpha", alpha, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's alpha value instantly then returns it to the provided alpha over time with FULL customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation. Identical to using ColorFrom and using the "a" parameter.
    /// </summary>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the initial alpha value of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the initial alpha value of the animation.
    /// </param>
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void FadeFrom(GameObject target, Hashtable args)
    {
        ColorFrom(target, args);
    }

    /// <summary>
    /// Changes a GameObject's alpha value over time with MINIMUM customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation. Identical to using ColorTo and using the "a" parameter.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void FadeTo(GameObject target, float alpha, float time)
    {
        FadeTo(target, Hash("alpha", alpha, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's alpha value over time with FULL customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation. Identical to using ColorTo and using the "a" parameter.
    /// </summary>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void FadeTo(GameObject target, Hashtable args)
    {
        ColorTo(target, args);
    }

    /// <summary>
    /// Changes a GameObject's color values instantly then returns them to the provided properties over time with MINIMUM customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ColorFrom(GameObject target, Color color, float time)
    {
        ColorFrom(target, Hash("color", color, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's color values instantly then returns them to the provided properties over time with FULL customization options.  If a GUIText or GUITexture component is attached, it will become the target of the animation.
    /// </summary>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="r">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
    /// </param>
    /// <param name="g">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="b">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="a">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="namedcolorvalue">
    /// A <see cref="NamedColorValue"/> or <see cref="System.String"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ColorFrom(GameObject target, Hashtable args)
    {
        Color fromColor = new Color();
        Color tempColor = new Color();

        //clean args:
        args = iTween.CleanArgs(args);

        //handle children:
        if (!args.Contains("includechildren") || (bool)args["includechildren"])
        {
            foreach (Transform child in target.transform)
            {
                Hashtable argsCopy = (Hashtable)args.Clone();
                argsCopy["ischild"] = true;
                ColorFrom(child.gameObject, argsCopy);
            }
        }

        //set a default easeType of linear if none is supplied since eased color interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //set tempColor and base fromColor:
        if (target.GetComponent<GUITexture>())
        {
            tempColor = fromColor = target.GetComponent<GUITexture>().color;
        }
        else if (target.GetComponent<GUIText>())
        {
            tempColor = fromColor = target.GetComponent<GUIText>().material.color;
        }
        else if (target.GetComponent<Renderer>())
        {
            tempColor = fromColor = target.GetComponent<Renderer>().material.color;
        }
        else if (target.GetComponent<Light>())
        {
            tempColor = fromColor = target.GetComponent<Light>().color;
        }

        //set augmented fromColor:
        if (args.Contains("color"))
        {
            fromColor = (Color)args["color"];
        }
        else
        {
            if (args.Contains("r"))
            {
                fromColor.r = (float)args["r"];
            }
            if (args.Contains("g"))
            {
                fromColor.g = (float)args["g"];
            }
            if (args.Contains("b"))
            {
                fromColor.b = (float)args["b"];
            }
            if (args.Contains("a"))
            {
                fromColor.a = (float)args["a"];
            }
        }

        //alpha or amount?
        if (args.Contains("amount"))
        {
            fromColor.a = (float)args["amount"];
            args.Remove("amount");
        }
        else if (args.Contains("alpha"))
        {
            fromColor.a = (float)args["alpha"];
            args.Remove("alpha");
        }

        //apply fromColor:
        if (target.GetComponent<GUITexture>())
        {
            target.GetComponent<GUITexture>().color = fromColor;
        }
        else if (target.GetComponent<GUIText>())
        {
            target.GetComponent<GUIText>().material.color = fromColor;
        }
        else if (target.GetComponent<Renderer>())
        {
            target.GetComponent<Renderer>().material.color = fromColor;
        }
        else if (target.GetComponent<Light>())
        {
            target.GetComponent<Light>().color = fromColor;
        }

        //set new color arg:
        args["color"] = tempColor;

        //establish iTween:
        args["type"] = "color";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Changes a GameObject's color values over time with MINIMUM customization options.  If a GUIText or GUITexture component is attached, they will become the target of the animation.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ColorTo(GameObject target, Color color, float time)
    {
        ColorTo(target, Hash("color", color, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's color values over time with FULL customization options.  If a GUIText or GUITexture component is attached, they will become the target of the animation.
    /// </summary>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="r">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
    /// </param>
    /// <param name="g">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="b">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="a">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="namedcolorvalue">
    /// A <see cref="NamedColorValue"/> or <see cref="System.String"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ColorTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //handle children:
        if (!args.Contains("includechildren") || (bool)args["includechildren"])
        {
            foreach (Transform child in target.transform)
            {
                Hashtable argsCopy = (Hashtable)args.Clone();
                argsCopy["ischild"] = true;
                ColorTo(child.gameObject, argsCopy);
            }
        }

        //set a default easeType of linear if none is supplied since eased color interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //establish iTween:
        args["type"] = "color";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Instantly changes an AudioSource's volume and pitch then returns it to it's starting volume and pitch over time with MINIMUM customization options. Default AudioSource attached to GameObject will be used (if one exists) if not supplied.
    /// </summary>
    /// <param name="target"> 
    /// A <see cref="GameObject"/> to be the target of the animation which holds the AudioSource to be changed.
    /// </param>
    /// <param name="volume"> for the target level of volume.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="pitch"> for the target pitch.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void AudioFrom(GameObject target, float volume, float pitch, float time)
    {
        AudioFrom(target, Hash("volume", volume, "pitch", pitch, "time", time));
    }

    /// <summary>
    /// Instantly changes an AudioSource's volume and pitch then returns it to it's starting volume and pitch over time with FULL customization options. Default AudioSource attached to GameObject will be used (if one exists) if not supplied. 
    /// </summary>
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use.
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void AudioFrom(GameObject target, Hashtable args)
    {
        Vector2 tempAudioProperties;
        Vector2 fromAudioProperties;
        AudioSource tempAudioSource;

        //clean args:
        args = iTween.CleanArgs(args);

        //set tempAudioSource:
        if (args.Contains("audiosource"))
        {
            tempAudioSource = (AudioSource)args["audiosource"];
        }
        else
        {
            if (target.GetComponent<AudioSource>())
            {
                tempAudioSource = target.GetComponent<AudioSource>();
            }
            else
            {
                //throw error if no AudioSource is available:
                Debug.LogError("iTween Error: AudioFrom requires an AudioSource.");
                return;
            }
        }

        //set tempAudioProperties:
        tempAudioProperties.x = fromAudioProperties.x = tempAudioSource.volume;
        tempAudioProperties.y = fromAudioProperties.y = tempAudioSource.pitch;

        //set augmented fromAudioProperties:
        if (args.Contains("volume"))
        {
            fromAudioProperties.x = (float)args["volume"];
        }
        if (args.Contains("pitch"))
        {
            fromAudioProperties.y = (float)args["pitch"];
        }

        //apply fromAudioProperties:
        tempAudioSource.volume = fromAudioProperties.x;
        tempAudioSource.pitch = fromAudioProperties.y;

        //set new volume and pitch args:
        args["volume"] = tempAudioProperties.x;
        args["pitch"] = tempAudioProperties.y;

        //set a default easeType of linear if none is supplied since eased audio interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //establish iTween:
        args["type"] = "audio";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Fades volume and pitch of an AudioSource with MINIMUM customization options.  Default AudioSource attached to GameObject will be used (if one exists) if not supplied. 
    /// </summary>
    /// <param name="target"> 
    /// A <see cref="GameObject"/> to be the target of the animation which holds the AudioSource to be changed.
    /// </param>
    /// <param name="volume"> for the target level of volume.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="pitch"> for the target pitch.
    /// A <see cref="System.Single"/>
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void AudioTo(GameObject target, float volume, float pitch, float time)
    {
        AudioTo(target, Hash("volume", volume, "pitch", pitch, "time", time));
    }

    /// <summary>
    /// Fades volume and pitch of an AudioSource with FULL customization options.  Default AudioSource attached to GameObject will be used (if one exists) if not supplied. 
    /// </summary>
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use.
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void AudioTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //set a default easeType of linear if none is supplied since eased audio interpolation is nearly unrecognizable:
        if (!args.Contains("easetype"))
        {
            args.Add("easetype", EaseType.Linear);
        }

        //establish iTween:
        args["type"] = "audio";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Plays an AudioClip once based on supplied volume and pitch and following any delay with MINIMUM customization options. AudioSource is optional as iTween will provide one.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation which holds the AudioSource to be utilized.
    /// </param>
    /// <param name="audioclip">
    /// A <see cref="AudioClip"/> for a reference to the AudioClip to be played.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> for the time in seconds the action will wait before beginning.
    /// </param>
    public static void Stab(GameObject target, AudioClip audioclip, float delay)
    {
        Stab(target, Hash("audioclip", audioclip, "delay", delay));
    }

    /// <summary>
    /// Plays an AudioClip once based on supplied volume and pitch and following any delay with FULL customization options. AudioSource is optional as iTween will provide one.
    /// </summary>
    /// <param name="audioclip">
    /// A <see cref="AudioClip"/> for a reference to the AudioClip to be played.
    /// </param> 
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the action will wait before beginning.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void Stab(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween:
        args["type"] = "stab";
        Launch(target, args);
    }

    /// <summary>
    /// Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time with MINIMUM customization options. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> to be the Vector3 that the target will look towards.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void LookFrom(GameObject target, Vector3 looktarget, float time)
    {
        LookFrom(target, Hash("looktarget", looktarget, "time", time));
    }

    /// <summary>
    /// Instantly rotates a GameObject to look at a supplied Transform or Vector3 then returns it to it's starting rotation over time with FULL customization options. 
    /// </summary>
    /// <param name="looktarget">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void LookFrom(GameObject target, Hashtable args)
    {
        Vector3 tempRotation;
        Vector3 tempRestriction;

        //clean args:
        args = iTween.CleanArgs(args);

        //set look:
        tempRotation = target.transform.eulerAngles;
        if (args["looktarget"].GetType() == typeof(Transform))
        {
            //target.transform.LookAt((Transform)args["looktarget"]);
            target.transform.LookAt((Transform)args["looktarget"], (Vector3?)args["up"] ?? Defaults.Up);
        }
        else if (args["looktarget"].GetType() == typeof(Vector3))
        {
            //target.transform.LookAt((Vector3)args["looktarget"]);
            target.transform.LookAt((Vector3)args["looktarget"], (Vector3?)args["up"] ?? Defaults.Up);
        }

        //axis restriction:
        if (args.Contains("axis"))
        {
            tempRestriction = target.transform.eulerAngles;
            switch ((string)args["axis"])
            {
                case "x":
                    tempRestriction.y = tempRotation.y;
                    tempRestriction.z = tempRotation.z;
                    break;
                case "y":
                    tempRestriction.x = tempRotation.x;
                    tempRestriction.z = tempRotation.z;
                    break;
                case "z":
                    tempRestriction.x = tempRotation.x;
                    tempRestriction.y = tempRotation.y;
                    break;
            }
            target.transform.eulerAngles = tempRestriction;
        }

        //set new rotation:
        args["rotation"] = tempRotation;

        //establish iTween
        args["type"] = "rotate";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Rotates a GameObject to look at the supplied Vector3 over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> to be the Vector3 that the target will look towards.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void LookTo(GameObject target, Vector3 looktarget, float time)
    {
        LookTo(target, Hash("looktarget", looktarget, "time", time));
    }

    /// <summary>
    /// Rotates a GameObject to look at a supplied Transform or Vector3 over time with FULL customization options.
    /// </summary>
    /// <param name="looktarget">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void LookTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("looktarget"))
        {
            if (args["looktarget"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["looktarget"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }

        //establish iTween
        args["type"] = "look";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Changes a GameObject's position over time to a supplied destination with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="position">
    /// A <see cref="Vector3"/> for the destination Vector3.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void MoveTo(GameObject target, Vector3 position, float time)
    {
        MoveTo(target, Hash("position", position, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's position over time to a supplied destination with FULL customization options.
    /// </summary>
    /// <param name="position">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/> or <see cref="Vector3[]"/> for a list of points to draw a Catmull-Rom through for a curved animation path.
    /// </param>
    /// <param name="movetopath">
    /// A <see cref="System.Boolean"/> for whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="lookahead">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how much of a percentage to look ahead on a path to influence how strict "orientopath" is.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void MoveTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("position"))
        {
            if (args["position"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["position"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        //establish iTween:
        args["type"] = "move";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="position">
    /// A <see cref="Vector3"/> for the destination Vector3.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void MoveFrom(GameObject target, Vector3 position, float time)
    {
        MoveFrom(target, Hash("position", position, "time", time));
    }

    /// <summary>
    /// Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time with FULL customization options.
    /// </summary>
    /// <param name="position">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/> or <see cref="Vector3[]"/> for a list of points to draw a Catmull-Rom through for a curved animation path.
    /// </param>
    /// <param name="movetopath">
    /// A <see cref="System.Boolean"/> for whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="lookahead">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for how much of a percentage to look ahead on a path to influence how strict "orientopath" is.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void MoveFrom(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        bool tempIsLocal;

        //set tempIsLocal:
        if (args.Contains("islocal"))
        {
            tempIsLocal = (bool)args["islocal"];
        }
        else
        {
            tempIsLocal = Defaults.IsLocal;
        }

        if (args.Contains("path"))
        {
            Vector3[] fromPath;
            Vector3[] suppliedPath;
            if (args["path"].GetType() == typeof(Vector3[]))
            {
                Vector3[] temp = (Vector3[])args["path"];
                suppliedPath = new Vector3[temp.Length];
                Array.Copy(temp, suppliedPath, temp.Length);
            }
            else
            {
                Transform[] temp = (Transform[])args["path"];
                suppliedPath = new Vector3[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    suppliedPath[i] = temp[i].position;
                }
            }
            if (suppliedPath[suppliedPath.Length - 1] != target.transform.position)
            {
                fromPath = new Vector3[suppliedPath.Length + 1];
                Array.Copy(suppliedPath, fromPath, suppliedPath.Length);
                if (tempIsLocal)
                {
                    fromPath[fromPath.Length - 1] = target.transform.localPosition;
                    target.transform.localPosition = fromPath[0];
                }
                else
                {
                    fromPath[fromPath.Length - 1] = target.transform.position;
                    target.transform.position = fromPath[0];
                }
                args["path"] = fromPath;
            }
            else
            {
                if (tempIsLocal)
                {
                    target.transform.localPosition = suppliedPath[0];
                }
                else
                {
                    target.transform.position = suppliedPath[0];
                }
                args["path"] = suppliedPath;
            }
        }
        else
        {
            Vector3 tempPosition;
            Vector3 fromPosition;

            //set tempPosition and base fromPosition:
            if (tempIsLocal)
            {
                tempPosition = fromPosition = target.transform.localPosition;
            }
            else
            {
                tempPosition = fromPosition = target.transform.position;
            }

            //set augmented fromPosition:
            if (args.Contains("position"))
            {
                if (args["position"].GetType() == typeof(Transform))
                {
                    Transform trans = (Transform)args["position"];
                    fromPosition = trans.position;
                }
                else if (args["position"].GetType() == typeof(Vector3))
                {
                    fromPosition = (Vector3)args["position"];
                }
            }
            else
            {
                if (args.Contains("x"))
                {
                    fromPosition.x = (float)args["x"];
                }
                if (args.Contains("y"))
                {
                    fromPosition.y = (float)args["y"];
                }
                if (args.Contains("z"))
                {
                    fromPosition.z = (float)args["z"];
                }
            }

            //apply fromPosition:
            if (tempIsLocal)
            {
                target.transform.localPosition = fromPosition;
            }
            else
            {
                target.transform.position = fromPosition;
            }

            //set new position arg:
            args["position"] = tempPosition;
        }

        //establish iTween:
        args["type"] = "move";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Translates a GameObject's position over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void MoveAdd(GameObject target, Vector3 amount, float time)
    {
        MoveAdd(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Translates a GameObject's position over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void MoveAdd(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween:
        args["type"] = "move";
        args["method"] = "add";
        Launch(target, args);
    }

    /// <summary>
    /// Adds the supplied coordinates to a GameObject's postion with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void MoveBy(GameObject target, Vector3 amount, float time)
    {
        MoveBy(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Adds the supplied coordinates to a GameObject's position with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of change in position to move the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void MoveBy(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween:
        args["type"] = "move";
        args["method"] = "by";
        Launch(target, args);
    }

    /// <summary>
    /// Changes a GameObject's scale over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="scale">
    /// A <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ScaleTo(GameObject target, Vector3 scale, float time)
    {
        ScaleTo(target, Hash("scale", scale, "time", time));
    }

    /// <summary>
    /// Changes a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="scale">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ScaleTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("scale"))
        {
            if (args["scale"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["scale"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Instantly changes a GameObject's scale then returns it to it's starting scale over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="scale">
    /// A <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ScaleFrom(GameObject target, Vector3 scale, float time)
    {
        ScaleFrom(target, Hash("scale", scale, "time", time));
    }

    /// <summary>
    /// Instantly changes a GameObject's scale then returns it to it's starting scale over time with FULL customization options.
    /// </summary>
    /// <param name="scale">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ScaleFrom(GameObject target, Hashtable args)
    {
        Vector3 tempScale;
        Vector3 fromScale;

        //clean args:
        args = iTween.CleanArgs(args);

        //set base fromScale:
        tempScale = fromScale = target.transform.localScale;

        //set augmented fromScale:
        if (args.Contains("scale"))
        {
            if (args["scale"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["scale"];
                fromScale = trans.localScale;
            }
            else if (args["scale"].GetType() == typeof(Vector3))
            {
                fromScale = (Vector3)args["scale"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                fromScale.x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                fromScale.y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                fromScale.z = (float)args["z"];
            }
        }

        //apply fromScale:
        target.transform.localScale = fromScale;

        //set new scale arg:
        args["scale"] = tempScale;

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Adds to a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of scale to be added to the GameObject's current scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ScaleAdd(GameObject target, Vector3 amount, float time)
    {
        ScaleAdd(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Adds to a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be added to the GameObject's current scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ScaleAdd(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "add";
        Launch(target, args);
    }

    /// <summary>
    /// Multiplies a GameObject's scale over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of scale to be multiplied by the GameObject's current scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ScaleBy(GameObject target, Vector3 amount, float time)
    {
        ScaleBy(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Multiplies a GameObject's scale over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be multiplied to the GameObject's current scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ScaleBy(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween:
        args["type"] = "scale";
        args["method"] = "by";
        Launch(target, args);
    }

    /// <summary>
    /// Rotates a GameObject to the supplied Euler angles in degrees over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="rotation">
    /// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void RotateTo(GameObject target, Vector3 rotation, float time)
    {
        RotateTo(target, Hash("rotation", rotation, "time", time));
    }

    /// <summary>
    /// Rotates a GameObject to the supplied Euler angles in degrees over time with FULL customization options.
    /// </summary>
    /// <param name="rotation">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void RotateTo(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //additional property to ensure ConflictCheck can work correctly since Transforms are refrences:		
        if (args.Contains("rotation"))
        {
            if (args["rotation"].GetType() == typeof(Transform))
            {
                Transform transform = (Transform)args["rotation"];
                args["position"] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                args["rotation"] = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                args["scale"] = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        //establish iTween
        args["type"] = "rotate";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time (if allowed) with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="rotation">
    /// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate from.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void RotateFrom(GameObject target, Vector3 rotation, float time)
    {
        RotateFrom(target, Hash("rotation", rotation, "time", time));
    }

    /// <summary>
    /// Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time (if allowed) with FULL customization options.
    /// </summary>
    /// <param name="rotation">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void RotateFrom(GameObject target, Hashtable args)
    {
        Vector3 tempRotation;
        Vector3 fromRotation;
        bool tempIsLocal;

        //clean args:
        args = iTween.CleanArgs(args);

        //set tempIsLocal:
        if (args.Contains("islocal"))
        {
            tempIsLocal = (bool)args["islocal"];
        }
        else
        {
            tempIsLocal = Defaults.IsLocal;
        }

        //set tempRotation and base fromRotation:
        if (tempIsLocal)
        {
            tempRotation = fromRotation = target.transform.localEulerAngles;
        }
        else
        {
            tempRotation = fromRotation = target.transform.eulerAngles;
        }

        //set augmented fromRotation:
        if (args.Contains("rotation"))
        {
            if (args["rotation"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["rotation"];
                fromRotation = trans.eulerAngles;
            }
            else if (args["rotation"].GetType() == typeof(Vector3))
            {
                fromRotation = (Vector3)args["rotation"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                fromRotation.x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                fromRotation.y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                fromRotation.z = (float)args["z"];
            }
        }

        //apply fromRotation:
        if (tempIsLocal)
        {
            target.transform.localEulerAngles = fromRotation;
        }
        else
        {
            target.transform.eulerAngles = fromRotation;
        }

        //set new rotation arg:
        args["rotation"] = tempRotation;

        //establish iTween:
        args["type"] = "rotate";
        args["method"] = "to";
        Launch(target, args);
    }

    /// <summary>
    /// Adds supplied Euler angles in degrees to a GameObject's rotation over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of Euler angles in degrees to add to the current rotation of the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void RotateAdd(GameObject target, Vector3 amount, float time)
    {
        RotateAdd(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Adds supplied Euler angles in degrees to a GameObject's rotation over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount of Euler angles in degrees to add to the current rotation of the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void RotateAdd(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween:
        args["type"] = "rotate";
        args["method"] = "add";
        Launch(target, args);
    }

    /// <summary>
    /// Multiplies supplied values by 360 and rotates a GameObject by calculated amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be multiplied by 360 to rotate the GameObject.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void RotateBy(GameObject target, Vector3 amount, float time)
    {
        RotateBy(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Multiplies supplied values by 360 and rotates a GameObject by calculated amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the amount to be multiplied by 360 to rotate the GameObject.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> or <see cref="System.String"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="speed">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> can be used instead of time to allow animation based on speed
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="easetype">
    /// A <see cref="EaseType"/> or <see cref="System.String"/> for the shape of the easing curve applied to the animation.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed.
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void RotateBy(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "rotate";
        args["method"] = "by";
        Launch(target, args);
    }

    /// <summary>
    /// Randomly shakes a GameObject's position by a diminishing amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ShakePosition(GameObject target, Vector3 amount, float time)
    {
        ShakePosition(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Randomly shakes a GameObject's position by a diminishing amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>  
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ShakePosition(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "shake";
        args["method"] = "position";
        Launch(target, args);
    }

    /// <summary>
    /// Randomly shakes a GameObject's scale by a diminishing amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ShakeScale(GameObject target, Vector3 amount, float time)
    {
        ShakeScale(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Randomly shakes a GameObject's scale by a diminishing amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ShakeScale(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "shake";
        args["method"] = "scale";
        Launch(target, args);
    }

    /// <summary>
    /// Randomly shakes a GameObject's rotation by a diminishing amount over time with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ShakeRotation(GameObject target, Vector3 amount, float time)
    {
        ShakeRotation(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Randomly shakes a GameObject's rotation by a diminishing amount over time with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param> 
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with shakes)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void ShakeRotation(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "shake";
        args["method"] = "rotation";
        Launch(target, args);
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's position and wobbles it back to its initial position with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of the punch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void PunchPosition(GameObject target, Vector3 amount, float time)
    {
        PunchPosition(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's position and wobbles it back to its initial position with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param> 
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget".
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param>   
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with punches)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void PunchPosition(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "punch";
        args["method"] = "position";
        args["easetype"] = EaseType.Punch;
        Launch(target, args);
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of the punch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void PunchRotation(GameObject target, Vector3 amount, float time)
    {
        PunchRotation(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="space">
    /// A <see cref="Space"/> for applying the transformation in either the world coordinate or local cordinate system. Defaults to local space.
    /// </param> 
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param> 
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with punches)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void PunchRotation(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "punch";
        args["method"] = "rotation";
        args["easetype"] = EaseType.Punch;
        Launch(target, args);
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale with MINIMUM customization options.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of the punch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void PunchScale(GameObject target, Vector3 amount, float time)
    {
        PunchScale(target, Hash("amount", amount, "time", time));
    }

    /// <summary>
    /// Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale with FULL customization options.
    /// </summary>
    /// <param name="amount">
    /// A <see cref="Vector3"/> for the magnitude of shake.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x magnitude.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y magnitude.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z magnitude.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    /// <param name="delay">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will wait before beginning.
    /// </param> 
    /// <param name="looptype">
    /// A <see cref="LoopType"/> or <see cref="System.String"/> for the type of loop to apply once the animation has completed. (only "loop" is allowed with punches)
    /// </param>
    /// <param name="onstart">
    /// A <see cref="System.String"/> for the name of a function to launch at the beginning of the animation.
    /// </param>
    /// <param name="onstarttarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onstart" method.
    /// </param>
    /// <param name="onstartparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onstart" method.
    /// </param>
    /// <param name="onupdate"> 
    /// A <see cref="System.String"/> for the name of a function to launch on every step of the animation.
    /// </param>
    /// <param name="onupdatetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "onupdate" method.
    /// </param>
    /// <param name="onupdateparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "onupdate" method.
    /// </param> 
    /// <param name="oncomplete">
    /// A <see cref="System.String"/> for the name of a function to launch at the end of the animation.
    /// </param>
    /// <param name="oncompletetarget">
    /// A <see cref="GameObject"/> for a reference to the GameObject that holds the "oncomplete" method.
    /// </param>
    /// <param name="oncompleteparams">
    /// A <see cref="System.Object"/> for arguments to be sent to the "oncomplete" method.
    /// </param>
    public static void PunchScale(GameObject target, Hashtable args)
    {
        //clean args:
        args = iTween.CleanArgs(args);

        //establish iTween
        args["type"] = "punch";
        args["method"] = "scale";
        args["easetype"] = EaseType.Punch;
        Launch(target, args);
    }

    #endregion

    #region #2 Generate Method Targets

    //call correct set target method and set tween application delegate:
    void GenerateTargets()
    {
        switch (Type)
        {
            case "value":
                switch (Method)
                {
                    case "float":
                        GenerateFloatTargets();
                        _apply = new ApplyTween(ApplyFloatTargets);
                        break;
                    case "vector2":
                        GenerateVector2Targets();
                        _apply = new ApplyTween(ApplyVector2Targets);
                        break;
                    case "vector3":
                        GenerateVector3Targets();
                        _apply = new ApplyTween(ApplyVector3Targets);
                        break;
                    case "color":
                        GenerateColorTargets();
                        _apply = new ApplyTween(ApplyColorTargets);
                        break;
                    case "rect":
                        GenerateRectTargets();
                        _apply = new ApplyTween(ApplyRectTargets);
                        break;
                }
                break;
            case "color":
                switch (Method)
                {
                    case "to":
                        GenerateColorToTargets();
                        _apply = new ApplyTween(ApplyColorToTargets);
                        break;
                }
                break;
            case "audio":
                switch (Method)
                {
                    case "to":
                        GenerateAudioToTargets();
                        _apply = new ApplyTween(ApplyAudioToTargets);
                        break;
                }
                break;
            case "move":
                switch (Method)
                {
                    case "to":
                        //using a path?
                        if (_tweenArguments.Contains("path"))
                        {
                            GenerateMoveToPathTargets();
                            _apply = new ApplyTween(ApplyMoveToPathTargets);
                        }
                        else
                        { //not using a path?
                            GenerateMoveToTargets();
                            _apply = new ApplyTween(ApplyMoveToTargets);
                        }
                        break;
                    case "by":
                    case "add":
                        GenerateMoveByTargets();
                        _apply = new ApplyTween(ApplyMoveByTargets);
                        break;
                }
                break;
            case "scale":
                switch (Method)
                {
                    case "to":
                        GenerateScaleToTargets();
                        _apply = new ApplyTween(ApplyScaleToTargets);
                        break;
                    case "by":
                        GenerateScaleByTargets();
                        _apply = new ApplyTween(ApplyScaleToTargets);
                        break;
                    case "add":
                        GenerateScaleAddTargets();
                        _apply = new ApplyTween(ApplyScaleToTargets);
                        break;
                }
                break;
            case "rotate":
                switch (Method)
                {
                    case "to":
                        GenerateRotateToTargets();
                        _apply = new ApplyTween(ApplyRotateToTargets);
                        break;
                    case "add":
                        GenerateRotateAddTargets();
                        _apply = new ApplyTween(ApplyRotateAddTargets);
                        break;
                    case "by":
                        GenerateRotateByTargets();
                        _apply = new ApplyTween(ApplyRotateAddTargets);
                        break;
                }
                break;
            case "shake":
                switch (Method)
                {
                    case "position":
                        GenerateShakePositionTargets();
                        _apply = new ApplyTween(ApplyShakePositionTargets);
                        break;
                    case "scale":
                        GenerateShakeScaleTargets();
                        _apply = new ApplyTween(ApplyShakeScaleTargets);
                        break;
                    case "rotation":
                        GenerateShakeRotationTargets();
                        _apply = new ApplyTween(ApplyShakeRotationTargets);
                        break;
                }
                break;
            case "punch":
                switch (Method)
                {
                    case "position":
                        GeneratePunchPositionTargets();
                        _apply = new ApplyTween(ApplyPunchPositionTargets);
                        break;
                    case "rotation":
                        GeneratePunchRotationTargets();
                        _apply = new ApplyTween(ApplyPunchRotationTargets);
                        break;
                    case "scale":
                        GeneratePunchScaleTargets();
                        _apply = new ApplyTween(ApplyPunchScaleTargets);
                        break;
                }
                break;
            case "look":
                switch (Method)
                {
                    case "to":
                        GenerateLookToTargets();
                        _apply = new ApplyTween(ApplyLookToTargets);
                        break;
                }
                break;
            case "stab":
                GenerateStabTargets();
                _apply = new ApplyTween(ApplyStabTargets);
                break;
        }
    }

    #endregion

    #region #3 Generate Specific Targets

    void GenerateRectTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _rects = new Rect[3];

        //from and to values:
        _rects[0] = (Rect)_tweenArguments["from"];
        _rects[1] = (Rect)_tweenArguments["to"];
    }

    void GenerateColorTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _colors = new Color[1, 3];

        //from and to values:
        _colors[0, 0] = (Color)_tweenArguments["from"];
        _colors[0, 1] = (Color)_tweenArguments["to"];
    }

    void GenerateVector3Targets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from and to values:
        _vector3S[0] = (Vector3)_tweenArguments["from"];
        _vector3S[1] = (Vector3)_tweenArguments["to"];

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateVector2Targets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector2S = new Vector2[3];

        //from and to values:
        _vector2S[0] = (Vector2)_tweenArguments["from"];
        _vector2S[1] = (Vector2)_tweenArguments["to"];

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            Vector3 fromV3 = new Vector3(_vector2S[0].x, _vector2S[0].y, 0);
            Vector3 toV3 = new Vector3(_vector2S[1].x, _vector2S[1].y, 0);
            float distance = Math.Abs(Vector3.Distance(fromV3, toV3));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateFloatTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _floats = new float[3];

        //from and to values:
        _floats[0] = (float)_tweenArguments["from"];
        _floats[1] = (float)_tweenArguments["to"];

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(_floats[0] - _floats[1]);
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateColorToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        //colors = new Color[3];

        //from and init to values:
        if (GetComponent<GUITexture>())
        {
            _colors = new Color[1, 3];
            _colors[0, 0] = _colors[0, 1] = GetComponent<GUITexture>().color;
        }
        else if (GetComponent<GUIText>())
        {
            _colors = new Color[1, 3];
            _colors[0, 0] = _colors[0, 1] = GetComponent<GUIText>().material.color;
        }
        else if (GetComponent<Renderer>())
        {
            _colors = new Color[GetComponent<Renderer>().materials.Length, 3];
            for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
            {
                _colors[i, 0] = GetComponent<Renderer>().materials[i].GetColor(_namedcolorvalue.ToString());
                _colors[i, 1] = GetComponent<Renderer>().materials[i].GetColor(_namedcolorvalue.ToString());
            }
            //colors[0] = colors[1] = renderer.material.color;	
        }
        else if (GetComponent<Light>())
        {
            _colors = new Color[1, 3];
            _colors[0, 0] = _colors[0, 1] = GetComponent<Light>().color;
        }
        else
        {
            _colors = new Color[1, 3]; //empty placeholder incase the GO is perhaps an empty holder or something similar
        }

        //to values:
        if (_tweenArguments.Contains("color"))
        {
            //colors[1]=(Color)tweenArguments["color"];
            for (int i = 0; i < _colors.GetLength(0); i++)
            {
                _colors[i, 1] = (Color)_tweenArguments["color"];
            }
        }
        else
        {
            if (_tweenArguments.Contains("r"))
            {
                //colors[1].r=(float)tweenArguments["r"];
                for (int i = 0; i < _colors.GetLength(0); i++)
                {
                    _colors[i, 1].r = (float)_tweenArguments["r"];
                }
            }
            if (_tweenArguments.Contains("g"))
            {
                //colors[1].g=(float)tweenArguments["g"];
                for (int i = 0; i < _colors.GetLength(0); i++)
                {
                    _colors[i, 1].g = (float)_tweenArguments["g"];
                }
            }
            if (_tweenArguments.Contains("b"))
            {
                //colors[1].b=(float)tweenArguments["b"];
                for (int i = 0; i < _colors.GetLength(0); i++)
                {
                    _colors[i, 1].b = (float)_tweenArguments["b"];
                }
            }
            if (_tweenArguments.Contains("a"))
            {
                //colors[1].a=(float)tweenArguments["a"];
                for (int i = 0; i < _colors.GetLength(0); i++)
                {
                    _colors[i, 1].a = (float)_tweenArguments["a"];
                }
            }
        }

        //alpha or amount?
        if (_tweenArguments.Contains("amount"))
        {
            //colors[1].a=(float)tweenArguments["amount"];
            for (int i = 0; i < _colors.GetLength(0); i++)
            {
                _colors[i, 1].a = (float)_tweenArguments["amount"];
            }
        }
        else if (_tweenArguments.Contains("alpha"))
        {
            //colors[1].a=(float)tweenArguments["alpha"];
            for (int i = 0; i < _colors.GetLength(0); i++)
            {
                _colors[i, 1].a = (float)_tweenArguments["alpha"];
            }
        }
    }

    void GenerateAudioToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector2S = new Vector2[3];

        //set audioSource:
        if (_tweenArguments.Contains("audiosource"))
        {
            _audioSource = (AudioSource)_tweenArguments["audiosource"];
        }
        else
        {
            if (GetComponent<AudioSource>())
            {
                _audioSource = GetComponent<AudioSource>();
            }
            else
            {
                //throw error if no AudioSource is available:
                Debug.LogError("iTween Error: AudioTo requires an AudioSource.");
                Dispose();
            }
        }

        //from values and default to values:
        _vector2S[0] = _vector2S[1] = new Vector2(_audioSource.volume, _audioSource.pitch);

        //to values:
        if (_tweenArguments.Contains("volume"))
        {
            _vector2S[1].x = (float)_tweenArguments["volume"];
        }
        if (_tweenArguments.Contains("pitch"))
        {
            _vector2S[1].y = (float)_tweenArguments["pitch"];
        }
    }

    void GenerateStabTargets()
    {
        //set audioSource:
        if (_tweenArguments.Contains("audiosource"))
        {
            _audioSource = (AudioSource)_tweenArguments["audiosource"];
        }
        else
        {
            if (GetComponent<AudioSource>())
            {
                _audioSource = GetComponent<AudioSource>();
            }
            else
            {
                //add and populate AudioSource if one doesn't exist:
                gameObject.AddComponent<AudioSource>();
                _audioSource = GetComponent<AudioSource>();
                _audioSource.playOnAwake = false;

            }
        }

        //populate audioSource's clip:
        _audioSource.clip = (AudioClip)_tweenArguments["audioclip"];

        //set audio's pitch and volume if requested:
        if (_tweenArguments.Contains("pitch"))
        {
            _audioSource.pitch = (float)_tweenArguments["pitch"];
        }
        if (_tweenArguments.Contains("volume"))
        {
            _audioSource.volume = (float)_tweenArguments["volume"];
        }

        //set run time based on length of clip after pitch is augmented
        Time = _audioSource.clip.length / _audioSource.pitch;
    }

    void GenerateLookToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        _vector3S[0] = _thisTransform.eulerAngles;

        //set look:
        if (_tweenArguments.Contains("looktarget"))
        {
            if (_tweenArguments["looktarget"].GetType() == typeof(Transform))
            {
                //transform.LookAt((Transform)tweenArguments["looktarget"]);
                _thisTransform.LookAt((Transform)_tweenArguments["looktarget"], (Vector3?)_tweenArguments["up"] ?? Defaults.Up);
            }
            else if (_tweenArguments["looktarget"].GetType() == typeof(Vector3))
            {
                //transform.LookAt((Vector3)tweenArguments["looktarget"]);
                _thisTransform.LookAt((Vector3)_tweenArguments["looktarget"], (Vector3?)_tweenArguments["up"] ?? Defaults.Up);
            }
        }
        else
        {
            Debug.LogError("iTween Error: LookTo needs a 'looktarget' property!");
            Dispose();
        }

        //to values:
        _vector3S[1] = _thisTransform.eulerAngles;
        _thisTransform.eulerAngles = _vector3S[0];

        //axis restriction:
        if (_tweenArguments.Contains("axis"))
        {
            switch ((string)_tweenArguments["axis"])
            {
                case "x":
                    _vector3S[1].y = _vector3S[0].y;
                    _vector3S[1].z = _vector3S[0].z;
                    break;
                case "y":
                    _vector3S[1].x = _vector3S[0].x;
                    _vector3S[1].z = _vector3S[0].z;
                    break;
                case "z":
                    _vector3S[1].x = _vector3S[0].x;
                    _vector3S[1].y = _vector3S[0].y;
                    break;
            }
        }

        //shortest distance:
        _vector3S[1] = new Vector3(Clerp(_vector3S[0].x, _vector3S[1].x, 1), Clerp(_vector3S[0].y, _vector3S[1].y, 1), Clerp(_vector3S[0].z, _vector3S[1].z, 1));

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateMoveToPathTargets()
    {
        Vector3[] suppliedPath;

        //create and store path points:
        if (_tweenArguments["path"].GetType() == typeof(Vector3[]))
        {
            Vector3[] temp = (Vector3[])_tweenArguments["path"];
            //if only one point is supplied fall back to MoveTo's traditional use since we can't have a curve with one value:
            if (temp.Length == 1)
            {
                Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
                Dispose();
            }
            suppliedPath = new Vector3[temp.Length];
            Array.Copy(temp, suppliedPath, temp.Length);
        }
        else
        {
            Transform[] temp = (Transform[])_tweenArguments["path"];
            //if only one point is supplied fall back to MoveTo's traditional use since we can't have a curve with one value:
            if (temp.Length == 1)
            {
                Debug.LogError("iTween Error: Attempting a path movement with MoveTo requires an array of more than 1 entry!");
                Dispose();
            }
            suppliedPath = new Vector3[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                suppliedPath[i] = temp[i].position;
            }
        }

        //do we need to plot a path to get to the beginning of the supplied path?		
        bool plotStart;
        int offset;
        if (_thisTransform.position != suppliedPath[0])
        {
            if (!_tweenArguments.Contains("movetopath") || (bool)_tweenArguments["movetopath"] == true)
            {
                plotStart = true;
                offset = 3;
            }
            else
            {
                plotStart = false;
                offset = 2;
            }
        }
        else
        {
            plotStart = false;
            offset = 2;
        }

        //build calculated path:
        _vector3S = new Vector3[suppliedPath.Length + offset];
        if (plotStart)
        {
            _vector3S[1] = _thisTransform.position;
            offset = 2;
        }
        else
        {
            offset = 1;
        }

        //populate calculate path;
        Array.Copy(suppliedPath, 0, _vector3S, offset, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        _vector3S[0] = _vector3S[1] + (_vector3S[1] - _vector3S[2]);
        _vector3S[_vector3S.Length - 1] = _vector3S[_vector3S.Length - 2] + (_vector3S[_vector3S.Length - 2] - _vector3S[_vector3S.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (_vector3S[1] == _vector3S[_vector3S.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[_vector3S.Length];
            Array.Copy(_vector3S, tmpLoopSpline, _vector3S.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            _vector3S = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, _vector3S, tmpLoopSpline.Length);
        }

        //create Catmull-Rom path:
        _path = new CrSpline(_vector3S);

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = PathLength(_vector3S);
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateMoveToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        if (_isLocal)
        {
            _vector3S[0] = _vector3S[1] = _thisTransform.localPosition;
        }
        else
        {
            _vector3S[0] = _vector3S[1] = _thisTransform.position;
        }

        //to values:
        if (_tweenArguments.Contains("position"))
        {
            if (_tweenArguments["position"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)_tweenArguments["position"];
                _vector3S[1] = trans.position;
            }
            else if (_tweenArguments["position"].GetType() == typeof(Vector3))
            {
                _vector3S[1] = (Vector3)_tweenArguments["position"];
            }
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }

        //handle orient to path request:
        if (_tweenArguments.Contains("orienttopath") && (bool)_tweenArguments["orienttopath"])
        {
            _tweenArguments["looktarget"] = _vector3S[1];
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateMoveByTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization, [4] original rotation to make sure look requests don't interfere with the direction object should move in, [5] for dial in location:
        _vector3S = new Vector3[6];

        //grab starting rotation:
        _vector3S[4] = _thisTransform.eulerAngles;

        //from values:
        _vector3S[0] = _vector3S[1] = _vector3S[3] = _thisTransform.position;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = _vector3S[0] + (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = _vector3S[0].x + (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = _vector3S[0].y + (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = _vector3S[0].z + (float)_tweenArguments["z"];
            }
        }

        //calculation for dial in:
        _thisTransform.Translate(_vector3S[1], _space);
        _vector3S[5] = _thisTransform.position;
        _thisTransform.position = _vector3S[0];

        //handle orient to path request:
        if (_tweenArguments.Contains("orienttopath") && (bool)_tweenArguments["orienttopath"])
        {
            _tweenArguments["looktarget"] = _vector3S[1];
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateScaleToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        _vector3S[0] = _vector3S[1] = _thisTransform.localScale;

        //to values:
        if (_tweenArguments.Contains("scale"))
        {
            if (_tweenArguments["scale"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)_tweenArguments["scale"];
                _vector3S[1] = trans.localScale;
            }
            else if (_tweenArguments["scale"].GetType() == typeof(Vector3))
            {
                _vector3S[1] = (Vector3)_tweenArguments["scale"];
            }
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateScaleByTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        _vector3S[0] = _vector3S[1] = _thisTransform.localScale;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = Vector3.Scale(_vector3S[1], (Vector3)_tweenArguments["amount"]);
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x *= (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y *= (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z *= (float)_tweenArguments["z"];
            }
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateScaleAddTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        _vector3S[0] = _vector3S[1] = _thisTransform.localScale;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] += (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x += (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y += (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z += (float)_tweenArguments["z"];
            }
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateRotateToTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        if (_isLocal)
        {
            _vector3S[0] = _vector3S[1] = _thisTransform.localEulerAngles;
        }
        else
        {
            _vector3S[0] = _vector3S[1] = _thisTransform.eulerAngles;
        }

        //to values:
        if (_tweenArguments.Contains("rotation"))
        {
            if (_tweenArguments["rotation"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)_tweenArguments["rotation"];
                _vector3S[1] = trans.eulerAngles;
            }
            else if (_tweenArguments["rotation"].GetType() == typeof(Vector3))
            {
                _vector3S[1] = (Vector3)_tweenArguments["rotation"];
            }
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }

        //shortest distance:
        _vector3S[1] = new Vector3(Clerp(_vector3S[0].x, _vector3S[1].x, 1), Clerp(_vector3S[0].y, _vector3S[1].y, 1), Clerp(_vector3S[0].z, _vector3S[1].z, 1));

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateRotateAddTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Rotate usage to allow Space utilization:
        _vector3S = new Vector3[5];

        //from values:
        _vector3S[0] = _vector3S[1] = _vector3S[3] = _thisTransform.eulerAngles;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] += (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x += (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y += (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z += (float)_tweenArguments["z"];
            }
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateRotateByTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Rotate usage to allow Space utilization:
        _vector3S = new Vector3[4];

        //from values:
        _vector3S[0] = _vector3S[1] = _vector3S[3] = _thisTransform.eulerAngles;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] += Vector3.Scale((Vector3)_tweenArguments["amount"], new Vector3(360, 360, 360));
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x += 360 * (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y += 360 * (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z += 360 * (float)_tweenArguments["z"];
            }
        }

        //need for speed?
        if (_tweenArguments.Contains("speed"))
        {
            float distance = Math.Abs(Vector3.Distance(_vector3S[0], _vector3S[1]));
            Time = distance / (float)_tweenArguments["speed"];
        }
    }

    void GenerateShakePositionTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] original rotation to make sure look requests don't interfere with the direction object should move in:
        _vector3S = new Vector3[4];

        //grab starting rotation:
        _vector3S[3] = _thisTransform.eulerAngles;

        //root:
        _vector3S[0] = _thisTransform.position;

        //amount:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }
    }

    void GenerateShakeScaleTargets()
    {
        //values holder [0] root value, [1] amount, [2] generated amount:
        _vector3S = new Vector3[3];

        //root:
        _vector3S[0] = _thisTransform.localScale;

        //amount:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }
    }

    void GenerateShakeRotationTargets()
    {
        //values holder [0] root value, [1] amount, [2] generated amount:
        _vector3S = new Vector3[3];

        //root:
        _vector3S[0] = _thisTransform.eulerAngles;

        //amount:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }
    }

    void GeneratePunchPositionTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization, [4] original rotation to make sure look requests don't interfere with the direction object should move in:
        _vector3S = new Vector3[5];

        //grab starting rotation:
        _vector3S[4] = _thisTransform.eulerAngles;

        //from values:
        _vector3S[0] = _thisTransform.position;
        _vector3S[1] = _vector3S[3] = Vector3.zero;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }
    }

    void GeneratePunchRotationTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation, [3] previous value for Translate usage to allow Space utilization:
        _vector3S = new Vector3[4];

        //from values:
        _vector3S[0] = _thisTransform.eulerAngles;
        _vector3S[1] = _vector3S[3] = Vector3.zero;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }
    }

    void GeneratePunchScaleTargets()
    {
        //values holder [0] from, [1] to, [2] calculated value from ease equation:
        _vector3S = new Vector3[3];

        //from values:
        _vector3S[0] = _thisTransform.localScale;
        _vector3S[1] = Vector3.zero;

        //to values:
        if (_tweenArguments.Contains("amount"))
        {
            _vector3S[1] = (Vector3)_tweenArguments["amount"];
        }
        else
        {
            if (_tweenArguments.Contains("x"))
            {
                _vector3S[1].x = (float)_tweenArguments["x"];
            }
            if (_tweenArguments.Contains("y"))
            {
                _vector3S[1].y = (float)_tweenArguments["y"];
            }
            if (_tweenArguments.Contains("z"))
            {
                _vector3S[1].z = (float)_tweenArguments["z"];
            }
        }
    }

    #endregion

    #region #4 Apply Targets

    void ApplyRectTargets()
    {
        //calculate:
        _rects[2].x = _ease(_rects[0].x, _rects[1].x, _percentage);
        _rects[2].y = _ease(_rects[0].y, _rects[1].y, _percentage);
        _rects[2].width = _ease(_rects[0].width, _rects[1].width, _percentage);
        _rects[2].height = _ease(_rects[0].height, _rects[1].height, _percentage);

        //apply:
        _tweenArguments["onupdateparams"] = _rects[2];

        //dial in:
        if (_percentage == 1)
        {
            _tweenArguments["onupdateparams"] = _rects[1];
        }
    }

    void ApplyColorTargets()
    {
        //calculate:
        _colors[0, 2].r = _ease(_colors[0, 0].r, _colors[0, 1].r, _percentage);
        _colors[0, 2].g = _ease(_colors[0, 0].g, _colors[0, 1].g, _percentage);
        _colors[0, 2].b = _ease(_colors[0, 0].b, _colors[0, 1].b, _percentage);
        _colors[0, 2].a = _ease(_colors[0, 0].a, _colors[0, 1].a, _percentage);

        //apply:
        _tweenArguments["onupdateparams"] = _colors[0, 2];

        //dial in:
        if (_percentage == 1)
        {
            _tweenArguments["onupdateparams"] = _colors[0, 1];
        }
    }

    void ApplyVector3Targets()
    {
        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:
        _tweenArguments["onupdateparams"] = _vector3S[2];

        //dial in:
        if (_percentage == 1)
        {
            _tweenArguments["onupdateparams"] = _vector3S[1];
        }
    }

    void ApplyVector2Targets()
    {
        //calculate:
        _vector2S[2].x = _ease(_vector2S[0].x, _vector2S[1].x, _percentage);
        _vector2S[2].y = _ease(_vector2S[0].y, _vector2S[1].y, _percentage);

        //apply:
        _tweenArguments["onupdateparams"] = _vector2S[2];

        //dial in:
        if (_percentage == 1)
        {
            _tweenArguments["onupdateparams"] = _vector2S[1];
        }
    }

    void ApplyFloatTargets()
    {
        //calculate:
        _floats[2] = _ease(_floats[0], _floats[1], _percentage);

        //apply:
        _tweenArguments["onupdateparams"] = _floats[2];

        //dial in:
        if (_percentage == 1)
        {
            _tweenArguments["onupdateparams"] = _floats[1];
        }
    }

    void ApplyColorToTargets()
    {
        //calculate:
        for (int i = 0; i < _colors.GetLength(0); i++)
        {
            _colors[i, 2].r = _ease(_colors[i, 0].r, _colors[i, 1].r, _percentage);
            _colors[i, 2].g = _ease(_colors[i, 0].g, _colors[i, 1].g, _percentage);
            _colors[i, 2].b = _ease(_colors[i, 0].b, _colors[i, 1].b, _percentage);
            _colors[i, 2].a = _ease(_colors[i, 0].a, _colors[i, 1].a, _percentage);
        }
        /*
		colors[2].r = ease(colors[0].r,colors[1].r,percentage);
		colors[2].g = ease(colors[0].g,colors[1].g,percentage);
		colors[2].b = ease(colors[0].b,colors[1].b,percentage);
		colors[2].a = ease(colors[0].a,colors[1].a,percentage);
		*/

        //apply:
        if (GetComponent<GUITexture>())
        {
            //guiTexture.color=colors[2];
            GetComponent<GUITexture>().color = _colors[0, 2];
        }
        else if (GetComponent<GUIText>())
        {
            //guiText.material.color=colors[2];
            GetComponent<GUIText>().material.color = _colors[0, 2];
        }
        else if (GetComponent<Renderer>())
        {
            //renderer.material.color=colors[2];
            for (int i = 0; i < _colors.GetLength(0); i++)
            {
                GetComponent<Renderer>().materials[i].SetColor(_namedcolorvalue.ToString(), _colors[i, 2]);
            }
        }
        else if (GetComponent<Light>())
        {
            //light.color=colors[2];	
            GetComponent<Light>().color = _colors[0, 2];
        }

        //dial in:
        if (_percentage == 1)
        {
            if (GetComponent<GUITexture>())
            {
                //guiTexture.color=colors[1];
                GetComponent<GUITexture>().color = _colors[0, 1];
            }
            else if (GetComponent<GUIText>())
            {
                //guiText.material.color=colors[1];
                GetComponent<GUIText>().material.color = _colors[0, 1];
            }
            else if (GetComponent<Renderer>())
            {
                //renderer.material.color=colors[1];	
                for (int i = 0; i < _colors.GetLength(0); i++)
                {
                    GetComponent<Renderer>().materials[i].SetColor(_namedcolorvalue.ToString(), _colors[i, 1]);
                }
            }
            else if (GetComponent<Light>())
            {
                //light.color=colors[1];	
                GetComponent<Light>().color = _colors[0, 1];
            }
        }
    }

    void ApplyAudioToTargets()
    {
        //calculate:
        _vector2S[2].x = _ease(_vector2S[0].x, _vector2S[1].x, _percentage);
        _vector2S[2].y = _ease(_vector2S[0].y, _vector2S[1].y, _percentage);

        //apply:
        _audioSource.volume = _vector2S[2].x;
        _audioSource.pitch = _vector2S[2].y;

        //dial in:
        if (_percentage == 1)
        {
            _audioSource.volume = _vector2S[1].x;
            _audioSource.pitch = _vector2S[1].y;
        }
    }

    void ApplyStabTargets()
    {
        //unnecessary but here just in case
    }

    void ApplyMoveToPathTargets()
    {
        _preUpdate = _thisTransform.position;
        float t = _ease(0, 1, _percentage);
        float lookAheadAmount;

        //clamp easing equation results as "back" will fail since overshoots aren't handled in the Catmull-Rom interpolation:
        if (_isLocal)
        {
            _thisTransform.localPosition = _path.Interp(Mathf.Clamp(t, 0, 1));
        }
        else
        {
            _thisTransform.position = _path.Interp(Mathf.Clamp(t, 0, 1));
        }

        //handle orient to path request:
        if (_tweenArguments.Contains("orienttopath") && (bool)_tweenArguments["orienttopath"])
        {

            //plot a point slightly ahead in the interpolation by pushing the percentage forward using the default lookahead value:
            float tLook;
            if (_tweenArguments.Contains("lookahead"))
            {
                lookAheadAmount = (float)_tweenArguments["lookahead"];
            }
            else
            {
                lookAheadAmount = Defaults.LookAhead;
            }
            //tLook = ease(0,1,percentage+lookAheadAmount);			
            tLook = _ease(0, 1, Mathf.Min(1f, _percentage + lookAheadAmount));

            //locate new leading point with a clamp as stated above:
            //Vector3 lookDistance = path.Interp(Mathf.Clamp(tLook,0,1)) - transform.position;
            _tweenArguments["looktarget"] = _path.Interp(Mathf.Clamp(tLook, 0, 1));
        }

        //need physics?
        _postUpdate = _thisTransform.position;
        if (_physics)
        {
            _thisTransform.position = _preUpdate;
            GetComponent<Rigidbody>().MovePosition(_postUpdate);
        }
    }

    void ApplyMoveToTargets()
    {
        //record current:
        _preUpdate = _thisTransform.position;


        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:	
        if (_isLocal)
        {
            _thisTransform.localPosition = _vector3S[2];
        }
        else
        {
            _thisTransform.position = _vector3S[2];
        }

        //dial in:
        if (_percentage == 1)
        {
            if (_isLocal)
            {
                _thisTransform.localPosition = _vector3S[1];
            }
            else
            {
                _thisTransform.position = _vector3S[1];
            }
        }

        //need physics?
        _postUpdate = _thisTransform.position;
        if (_physics)
        {
            _thisTransform.position = _preUpdate;
            GetComponent<Rigidbody>().MovePosition(_postUpdate);
        }
    }

    void ApplyMoveByTargets()
    {
        _preUpdate = _thisTransform.position;

        //reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
        Vector3 currentRotation = new Vector3();

        if (_tweenArguments.Contains("looktarget"))
        {
            currentRotation = _thisTransform.eulerAngles;
            _thisTransform.eulerAngles = _vector3S[4];
        }

        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:
        _thisTransform.Translate(_vector3S[2] - _vector3S[3], _space);

        //record:
        _vector3S[3] = _vector3S[2];

        //reset rotation:
        if (_tweenArguments.Contains("looktarget"))
        {
            _thisTransform.eulerAngles = currentRotation;
        }

        /*
		//dial in:
		if(percentage==1){	
			transform.position=vector3s[5];
		}
		*/

        //need physics?
        _postUpdate = _thisTransform.position;
        if (_physics)
        {
            _thisTransform.position = _preUpdate;
            GetComponent<Rigidbody>().MovePosition(_postUpdate);
        }
    }

    void ApplyScaleToTargets()
    {
        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:
        _thisTransform.localScale = _vector3S[2];

        //dial in:
        if (_percentage == 1)
        {
            _thisTransform.localScale = _vector3S[1];
        }
    }

    void ApplyLookToTargets()
    {
        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:
        if (_isLocal)
        {
            _thisTransform.localRotation = Quaternion.Euler(_vector3S[2]);
        }
        else
        {
            _thisTransform.rotation = Quaternion.Euler(_vector3S[2]);
        };
    }

    void ApplyRotateToTargets()
    {
        _preUpdate = _thisTransform.eulerAngles;

        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:
        if (_isLocal)
        {
            _thisTransform.localRotation = Quaternion.Euler(_vector3S[2]);
        }
        else
        {
            _thisTransform.rotation = Quaternion.Euler(_vector3S[2]);
        };

        //dial in:
        if (_percentage == 1)
        {
            if (_isLocal)
            {
                _thisTransform.localRotation = Quaternion.Euler(_vector3S[1]);
            }
            else
            {
                _thisTransform.rotation = Quaternion.Euler(_vector3S[1]);
            };
        }

        //need physics?
        _postUpdate = _thisTransform.eulerAngles;
        if (_physics)
        {
            _thisTransform.eulerAngles = _preUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(_postUpdate));
        }
    }

    void ApplyRotateAddTargets()
    {
        _preUpdate = _thisTransform.eulerAngles;

        //calculate:
        _vector3S[2].x = _ease(_vector3S[0].x, _vector3S[1].x, _percentage);
        _vector3S[2].y = _ease(_vector3S[0].y, _vector3S[1].y, _percentage);
        _vector3S[2].z = _ease(_vector3S[0].z, _vector3S[1].z, _percentage);

        //apply:
        _thisTransform.Rotate(_vector3S[2] - _vector3S[3], _space);

        //record:
        _vector3S[3] = _vector3S[2];

        //need physics?
        _postUpdate = _thisTransform.eulerAngles;
        if (_physics)
        {
            _thisTransform.eulerAngles = _preUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(_postUpdate));
        }
    }

    void ApplyShakePositionTargets()
    {
        //preUpdate = transform.position;
        if (_isLocal)
        {
            _preUpdate = _thisTransform.localPosition;
        }
        else
        {
            _preUpdate = _thisTransform.position;
        }

        //reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
        Vector3 currentRotation = new Vector3();

        if (_tweenArguments.Contains("looktarget"))
        {
            currentRotation = _thisTransform.eulerAngles;
            _thisTransform.eulerAngles = _vector3S[3];
        }

        //impact:
        if (_percentage == 0)
        {
            _thisTransform.Translate(_vector3S[1], _space);
        }

        //transform.position=vector3s[0];
        //reset:
        if (_isLocal)
        {
            _thisTransform.localPosition = _vector3S[0];
        }
        else
        {
            _thisTransform.position = _vector3S[0];
        }

        //generate:
        float diminishingControl = 1 - _percentage;
        _vector3S[2].x = UnityEngine.Random.Range(-_vector3S[1].x * diminishingControl, _vector3S[1].x * diminishingControl);
        _vector3S[2].y = UnityEngine.Random.Range(-_vector3S[1].y * diminishingControl, _vector3S[1].y * diminishingControl);
        _vector3S[2].z = UnityEngine.Random.Range(-_vector3S[1].z * diminishingControl, _vector3S[1].z * diminishingControl);

        //apply:	
        //transform.Translate(vector3s[2],space);	
        if (_isLocal)
        {
            _thisTransform.localPosition += _vector3S[2];
        }
        else
        {
            _thisTransform.position += _vector3S[2];
        }

        //reset rotation:
        if (_tweenArguments.Contains("looktarget"))
        {
            _thisTransform.eulerAngles = currentRotation;
        }

        //need physics?
        _postUpdate = _thisTransform.position;
        if (_physics)
        {
            _thisTransform.position = _preUpdate;
            GetComponent<Rigidbody>().MovePosition(_postUpdate);
        }
    }

    void ApplyShakeScaleTargets()
    {
        //impact:
        if (_percentage == 0)
        {
            _thisTransform.localScale = _vector3S[1];
        }

        //reset:
        _thisTransform.localScale = _vector3S[0];

        //generate:
        float diminishingControl = 1 - _percentage;
        _vector3S[2].x = UnityEngine.Random.Range(-_vector3S[1].x * diminishingControl, _vector3S[1].x * diminishingControl);
        _vector3S[2].y = UnityEngine.Random.Range(-_vector3S[1].y * diminishingControl, _vector3S[1].y * diminishingControl);
        _vector3S[2].z = UnityEngine.Random.Range(-_vector3S[1].z * diminishingControl, _vector3S[1].z * diminishingControl);

        //apply:
        _thisTransform.localScale += _vector3S[2];
    }

    void ApplyShakeRotationTargets()
    {
        _preUpdate = _thisTransform.eulerAngles;

        //impact:
        if (_percentage == 0)
        {
            _thisTransform.Rotate(_vector3S[1], _space);
        }

        //reset:
        _thisTransform.eulerAngles = _vector3S[0];

        //generate:
        float diminishingControl = 1 - _percentage;
        _vector3S[2].x = UnityEngine.Random.Range(-_vector3S[1].x * diminishingControl, _vector3S[1].x * diminishingControl);
        _vector3S[2].y = UnityEngine.Random.Range(-_vector3S[1].y * diminishingControl, _vector3S[1].y * diminishingControl);
        _vector3S[2].z = UnityEngine.Random.Range(-_vector3S[1].z * diminishingControl, _vector3S[1].z * diminishingControl);

        //apply:
        _thisTransform.Rotate(_vector3S[2], _space);

        //need physics?
        _postUpdate = _thisTransform.eulerAngles;
        if (_physics)
        {
            _thisTransform.eulerAngles = _preUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(_postUpdate));
        }
    }

    void ApplyPunchPositionTargets()
    {
        _preUpdate = _thisTransform.position;

        //reset rotation to prevent look interferences as object rotates and attempts to move with translate and record current rotation
        Vector3 currentRotation = new Vector3();

        if (_tweenArguments.Contains("looktarget"))
        {
            currentRotation = _thisTransform.eulerAngles;
            _thisTransform.eulerAngles = _vector3S[4];
        }

        //calculate:
        if (_vector3S[1].x > 0)
        {
            _vector3S[2].x = Punch(_vector3S[1].x, _percentage);
        }
        else if (_vector3S[1].x < 0)
        {
            _vector3S[2].x = -Punch(Mathf.Abs(_vector3S[1].x), _percentage);
        }
        if (_vector3S[1].y > 0)
        {
            _vector3S[2].y = Punch(_vector3S[1].y, _percentage);
        }
        else if (_vector3S[1].y < 0)
        {
            _vector3S[2].y = -Punch(Mathf.Abs(_vector3S[1].y), _percentage);
        }
        if (_vector3S[1].z > 0)
        {
            _vector3S[2].z = Punch(_vector3S[1].z, _percentage);
        }
        else if (_vector3S[1].z < 0)
        {
            _vector3S[2].z = -Punch(Mathf.Abs(_vector3S[1].z), _percentage);
        }

        //apply:
        _thisTransform.Translate(_vector3S[2] - _vector3S[3], _space);

        //record:
        _vector3S[3] = _vector3S[2];

        //reset rotation:
        if (_tweenArguments.Contains("looktarget"))
        {
            _thisTransform.eulerAngles = currentRotation;
        }

        //dial in:
        /*
		if(percentage==1){	
			transform.position=vector3s[0];
		}
		*/

        //need physics?
        _postUpdate = _thisTransform.position;
        if (_physics)
        {
            _thisTransform.position = _preUpdate;
            GetComponent<Rigidbody>().MovePosition(_postUpdate);
        }
    }

    void ApplyPunchRotationTargets()
    {
        _preUpdate = _thisTransform.eulerAngles;

        //calculate:
        if (_vector3S[1].x > 0)
        {
            _vector3S[2].x = Punch(_vector3S[1].x, _percentage);
        }
        else if (_vector3S[1].x < 0)
        {
            _vector3S[2].x = -Punch(Mathf.Abs(_vector3S[1].x), _percentage);
        }
        if (_vector3S[1].y > 0)
        {
            _vector3S[2].y = Punch(_vector3S[1].y, _percentage);
        }
        else if (_vector3S[1].y < 0)
        {
            _vector3S[2].y = -Punch(Mathf.Abs(_vector3S[1].y), _percentage);
        }
        if (_vector3S[1].z > 0)
        {
            _vector3S[2].z = Punch(_vector3S[1].z, _percentage);
        }
        else if (_vector3S[1].z < 0)
        {
            _vector3S[2].z = -Punch(Mathf.Abs(_vector3S[1].z), _percentage);
        }

        //apply:
        _thisTransform.Rotate(_vector3S[2] - _vector3S[3], _space);

        //record:
        _vector3S[3] = _vector3S[2];

        //dial in:
        /*
		if(percentage==1){	
			transform.eulerAngles=vector3s[0];
		}
		*/

        //need physics?
        _postUpdate = _thisTransform.eulerAngles;
        if (_physics)
        {
            _thisTransform.eulerAngles = _preUpdate;
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(_postUpdate));
        }
    }

    void ApplyPunchScaleTargets()
    {
        //calculate:
        if (_vector3S[1].x > 0)
        {
            _vector3S[2].x = Punch(_vector3S[1].x, _percentage);
        }
        else if (_vector3S[1].x < 0)
        {
            _vector3S[2].x = -Punch(Mathf.Abs(_vector3S[1].x), _percentage);
        }
        if (_vector3S[1].y > 0)
        {
            _vector3S[2].y = Punch(_vector3S[1].y, _percentage);
        }
        else if (_vector3S[1].y < 0)
        {
            _vector3S[2].y = -Punch(Mathf.Abs(_vector3S[1].y), _percentage);
        }
        if (_vector3S[1].z > 0)
        {
            _vector3S[2].z = Punch(_vector3S[1].z, _percentage);
        }
        else if (_vector3S[1].z < 0)
        {
            _vector3S[2].z = -Punch(Mathf.Abs(_vector3S[1].z), _percentage);
        }

        //apply:
        _thisTransform.localScale = _vector3S[0] + _vector3S[2];

        //dial in:
        /*
		if(percentage==1){	
			transform.localScale=vector3s[0];
		}
		*/
    }

    #endregion

    #region #5 Tween Steps

    IEnumerator TweenDelay()
    {
        _delayStarted = UnityEngine.Time.time;
        yield return new WaitForSeconds(Delay);
        if (_wasPaused)
        {
            _wasPaused = false;
            TweenStart();
        }
    }

    void TweenStart()
    {
        CallBack("onstart");

        if (!_loop)
        {//only if this is not a loop
            ConflictCheck();
            GenerateTargets();
        }

        //run stab:
        if (Type == "stab")
        {
            _audioSource.PlayOneShot(_audioSource.clip);
        }

        //toggle isKinematic for iTweens that may interfere with physics:
        if (Type == "move" || Type == "scale" || Type == "rotate" || Type == "punch" || Type == "shake" || Type == "curve" || Type == "look")
        {
            EnableKinematic();
        }

        IsRunning = true;
    }

    IEnumerator TweenRestart()
    {
        if (Delay > 0)
        {
            _delayStarted = UnityEngine.Time.time;
            yield return new WaitForSeconds(Delay);
        }
        _loop = true;
        TweenStart();
    }

    void TweenUpdate()
    {
        _apply();
        CallBack("onupdate");
        UpdatePercentage();
    }

    void TweenComplete()
    {
        IsRunning = false;

        //dial in percentage to 1 or 0 for final run:
        if (_percentage > .5f)
        {
            _percentage = 1f;
        }
        else
        {
            _percentage = 0;
        }

        //apply dial in and final run:
        _apply();
        if (Type == "value")
        {
            CallBack("onupdate"); //CallBack run for ValueTo since it only calculates and applies in the update callback
        }

        //loop or dispose?
        if (loopType == LoopType.None)
        {
            Dispose();
        }
        else
        {
            TweenLoop();
        }

        CallBack("oncomplete");
    }

    void TweenLoop()
    {
        DisableKinematic(); //give physics control again
        switch (loopType)
        {
            case LoopType.Loop:
                //rewind:
                _percentage = 0;
                _runningTime = 0;
                _apply();

                //replay:
                StartCoroutine("TweenRestart");
                break;
            case LoopType.PingPong:
                _reverse = !_reverse;
                _runningTime = 0;

                //replay:
                StartCoroutine("TweenRestart");
                break;
        }
    }

    #endregion

    #region #6 Update Callable

    /// <summary>
    /// Returns a Rect that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="Rect"/
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="Rect"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="Rect"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static Rect RectUpdate(Rect currentValue, Rect targetValue, float speed)
    {
        Rect diff = new Rect(FloatUpdate(currentValue.x, targetValue.x, speed), FloatUpdate(currentValue.y, targetValue.y, speed), FloatUpdate(currentValue.width, targetValue.width, speed), FloatUpdate(currentValue.height, targetValue.height, speed));
        return (diff);
    }

    /// <summary>
    /// Returns a Vector3 that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector3"/>
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="Vector3"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="Vector3"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static Vector3 Vector3Update(Vector3 currentValue, Vector3 targetValue, float speed)
    {
        Vector3 diff = targetValue - currentValue;
        currentValue += (diff * speed) * UnityEngine.Time.deltaTime;
        return (currentValue);
    }

    /// <summary>
    /// Returns a Vector2 that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector2"/>
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="Vector2"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="Vector2"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static Vector2 Vector2Update(Vector2 currentValue, Vector2 targetValue, float speed)
    {
        Vector2 diff = targetValue - currentValue;
        currentValue += (diff * speed) * UnityEngine.Time.deltaTime;
        return (currentValue);
    }

    /// <summary>
    /// Returns a float that is eased between a current and target value by the supplied speed.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Single"/>
    /// </returns>
    /// <param name='currentValue'>
    /// A <see cref="System.Single"/> the starting or initial value
    /// </param>
    /// <param name='targetValue'>
    /// A <see cref="System.Single"/> the target value that the current value will be eased to.
    /// </param>
    /// <param name='speed'>
    /// A <see cref="System.Single"/> to be used as rate of speed (larger number equals faster animation)
    /// </param>
    public static float FloatUpdate(float currentValue, float targetValue, float speed)
    {
        float diff = targetValue - currentValue;
        currentValue += (diff * speed) * UnityEngine.Time.deltaTime;
        return (currentValue);
    }

    /// <summary>
    /// Similar to FadeTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void FadeUpdate(GameObject target, Hashtable args)
    {
        args["a"] = args["alpha"];
        ColorUpdate(target, args);
    }

    /// <summary>
    /// Similar to FadeTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="alpha">
    /// A <see cref="System.Single"/> for the final alpha value of the animation.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void FadeUpdate(GameObject target, float alpha, float time)
    {
        FadeUpdate(target, Hash("alpha", alpha, "time", time));
    }

    /// <summary>
    /// Similar to ColorTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="r">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color red.
    /// </param>
    /// <param name="g">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="b">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the color green.
    /// </param>
    /// <param name="a">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="namedcolorvalue">
    /// A <see cref="NamedColorValue"/> or <see cref="System.String"/> for the individual setting of the alpha.
    /// </param> 
    /// <param name="includechildren">
    /// A <see cref="System.Boolean"/> for whether or not to include children of this GameObject. True by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ColorUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Color[] colors = new Color[4];

        //handle children:
        if (!args.Contains("includechildren") || (bool)args["includechildren"])
        {
            foreach (Transform child in target.transform)
            {
                ColorUpdate(child.gameObject, args);
            }
        }

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.UpdateTimePercentage;
        }
        else
        {
            time = Defaults.UpdateTime;
        }

        //init values:
        if (target.GetComponent<GUITexture>())
        {
            colors[0] = colors[1] = target.GetComponent<GUITexture>().color;
        }
        else if (target.GetComponent<GUIText>())
        {
            colors[0] = colors[1] = target.GetComponent<GUIText>().material.color;
        }
        else if (target.GetComponent<Renderer>())
        {
            colors[0] = colors[1] = target.GetComponent<Renderer>().material.color;
        }
        else if (target.GetComponent<Light>())
        {
            colors[0] = colors[1] = target.GetComponent<Light>().color;
        }

        //to values:
        if (args.Contains("color"))
        {
            colors[1] = (Color)args["color"];
        }
        else
        {
            if (args.Contains("r"))
            {
                colors[1].r = (float)args["r"];
            }
            if (args.Contains("g"))
            {
                colors[1].g = (float)args["g"];
            }
            if (args.Contains("b"))
            {
                colors[1].b = (float)args["b"];
            }
            if (args.Contains("a"))
            {
                colors[1].a = (float)args["a"];
            }
        }

        //calculate:
        colors[3].r = Mathf.SmoothDamp(colors[0].r, colors[1].r, ref colors[2].r, time);
        colors[3].g = Mathf.SmoothDamp(colors[0].g, colors[1].g, ref colors[2].g, time);
        colors[3].b = Mathf.SmoothDamp(colors[0].b, colors[1].b, ref colors[2].b, time);
        colors[3].a = Mathf.SmoothDamp(colors[0].a, colors[1].a, ref colors[2].a, time);

        //apply:
        if (target.GetComponent<GUITexture>())
        {
            target.GetComponent<GUITexture>().color = colors[3];
        }
        else if (target.GetComponent<GUIText>())
        {
            target.GetComponent<GUIText>().material.color = colors[3];
        }
        else if (target.GetComponent<Renderer>())
        {
            target.GetComponent<Renderer>().material.color = colors[3];
        }
        else if (target.GetComponent<Light>())
        {
            target.GetComponent<Light>().color = colors[3];
        }
    }

    /// <summary>
    /// Similar to ColorTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/> to change the GameObject's color to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ColorUpdate(GameObject target, Color color, float time)
    {
        ColorUpdate(target, Hash("color", color, "time", time));
    }

    /// <summary>
    /// Similar to AudioTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="audiosource">
    /// A <see cref="AudioSource"/> for which AudioSource to use.
    /// </param> 
    /// <param name="volume">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void AudioUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        AudioSource audioSource;
        float time;
        Vector2[] vector2S = new Vector2[4];

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.UpdateTimePercentage;
        }
        else
        {
            time = Defaults.UpdateTime;
        }

        //set audioSource:
        if (args.Contains("audiosource"))
        {
            audioSource = (AudioSource)args["audiosource"];
        }
        else
        {
            if (target.GetComponent<AudioSource>())
            {
                audioSource = target.GetComponent<AudioSource>();
            }
            else
            {
                //throw error if no AudioSource is available:
                Debug.LogError("iTween Error: AudioUpdate requires an AudioSource.");
                return;
            }
        }

        //from values:
        vector2S[0] = vector2S[1] = new Vector2(audioSource.volume, audioSource.pitch);

        //set to:
        if (args.Contains("volume"))
        {
            vector2S[1].x = (float)args["volume"];
        }
        if (args.Contains("pitch"))
        {
            vector2S[1].y = (float)args["pitch"];
        }

        //calculate:
        vector2S[3].x = Mathf.SmoothDampAngle(vector2S[0].x, vector2S[1].x, ref vector2S[2].x, time);
        vector2S[3].y = Mathf.SmoothDampAngle(vector2S[0].y, vector2S[1].y, ref vector2S[2].y, time);

        //apply:
        audioSource.volume = vector2S[3].x;
        audioSource.pitch = vector2S[3].y;
    }

    /// <summary>
    /// Similar to AudioTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="volume">
    /// A <see cref="System.Single"/> for the target level of volume.
    /// </param>
    /// <param name="pitch">
    /// A <see cref="System.Single"/> for the target pitch.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void AudioUpdate(GameObject target, float volume, float pitch, float time)
    {
        AudioUpdate(target, Hash("volume", volume, "pitch", pitch, "time", time));
    }

    /// <summary>
    /// Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="rotation">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    public static void RotateUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        bool isLocal;
        float time;
        Vector3[] vector3S = new Vector3[4];
        Vector3 preUpdate = target.transform.eulerAngles;

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.UpdateTimePercentage;
        }
        else
        {
            time = Defaults.UpdateTime;
        }

        //set isLocal:
        if (args.Contains("islocal"))
        {
            isLocal = (bool)args["islocal"];
        }
        else
        {
            isLocal = Defaults.IsLocal;
        }

        //from values:
        if (isLocal)
        {
            vector3S[0] = target.transform.localEulerAngles;
        }
        else
        {
            vector3S[0] = target.transform.eulerAngles;
        }

        //set to:
        if (args.Contains("rotation"))
        {
            if (args["rotation"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["rotation"];
                vector3S[1] = trans.eulerAngles;
            }
            else if (args["rotation"].GetType() == typeof(Vector3))
            {
                vector3S[1] = (Vector3)args["rotation"];
            }
        }

        //calculate:
        vector3S[3].x = Mathf.SmoothDampAngle(vector3S[0].x, vector3S[1].x, ref vector3S[2].x, time);
        vector3S[3].y = Mathf.SmoothDampAngle(vector3S[0].y, vector3S[1].y, ref vector3S[2].y, time);
        vector3S[3].z = Mathf.SmoothDampAngle(vector3S[0].z, vector3S[1].z, ref vector3S[2].z, time);

        //apply:
        if (isLocal)
        {
            target.transform.localEulerAngles = vector3S[3];
        }
        else
        {
            target.transform.eulerAngles = vector3S[3];
        }

        //need physics?
        if (target.GetComponent<Rigidbody>() != null)
        {
            Vector3 postUpdate = target.transform.eulerAngles;
            target.transform.eulerAngles = preUpdate;
            target.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(postUpdate));
        }
    }

    /// <summary>
    /// Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="rotation">
    /// A <see cref="Vector3"/> for the target Euler angles in degrees to rotate to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void RotateUpdate(GameObject target, Vector3 rotation, float time)
    {
        RotateUpdate(target, Hash("rotation", rotation, "time", time));
    }

    /// <summary>
    /// Similar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options.  Does not utilize an EaseType. 
    /// </summary>
    /// <param name="scale">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    public static void ScaleUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Vector3[] vector3S = new Vector3[4];

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.UpdateTimePercentage;
        }
        else
        {
            time = Defaults.UpdateTime;
        }

        //init values:
        vector3S[0] = vector3S[1] = target.transform.localScale;

        //to values:
        if (args.Contains("scale"))
        {
            if (args["scale"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["scale"];
                vector3S[1] = trans.localScale;
            }
            else if (args["scale"].GetType() == typeof(Vector3))
            {
                vector3S[1] = (Vector3)args["scale"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                vector3S[1].x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                vector3S[1].y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                vector3S[1].z = (float)args["z"];
            }
        }

        //calculate:
        vector3S[3].x = Mathf.SmoothDamp(vector3S[0].x, vector3S[1].x, ref vector3S[2].x, time);
        vector3S[3].y = Mathf.SmoothDamp(vector3S[0].y, vector3S[1].y, ref vector3S[2].y, time);
        vector3S[3].z = Mathf.SmoothDamp(vector3S[0].z, vector3S[1].z, ref vector3S[2].z, time);

        //apply:
        target.transform.localScale = vector3S[3];
    }

    /// <summary>
    /// Similar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options.  Does not utilize an EaseType.
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="scale">
    /// A <see cref="Vector3"/> for the final scale.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void ScaleUpdate(GameObject target, Vector3 scale, float time)
    {
        ScaleUpdate(target, Hash("scale", scale, "time", time));
    }

    /// <summary>
    /// Similar to MoveTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="position">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="x">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the x axis.
    /// </param>
    /// <param name="y">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the y axis.
    /// </param>
    /// <param name="z">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the individual setting of the z axis.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    /// <param name="islocal">
    /// A <see cref="System.Boolean"/> for whether to animate in world space or relative to the parent. False by default.
    /// </param>
    /// <param name="orienttopath">
    /// A <see cref="System.Boolean"/> for whether or not the GameObject will orient to its direction of travel.  False by default.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> or A <see cref="Transform"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="looktime">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the object will take to look at either the "looktarget" or "orienttopath".
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    public static void MoveUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Vector3[] vector3S = new Vector3[4];
        bool isLocal;
        Vector3 preUpdate = target.transform.position;

        //set smooth time:
        if (args.Contains("time"))
        {
            time = (float)args["time"];
            time *= Defaults.UpdateTimePercentage;
        }
        else
        {
            time = Defaults.UpdateTime;
        }

        //set isLocal:
        if (args.Contains("islocal"))
        {
            isLocal = (bool)args["islocal"];
        }
        else
        {
            isLocal = Defaults.IsLocal;
        }

        //init values:
        if (isLocal)
        {
            vector3S[0] = vector3S[1] = target.transform.localPosition;
        }
        else
        {
            vector3S[0] = vector3S[1] = target.transform.position;
        }

        //to values:
        if (args.Contains("position"))
        {
            if (args["position"].GetType() == typeof(Transform))
            {
                Transform trans = (Transform)args["position"];
                vector3S[1] = trans.position;
            }
            else if (args["position"].GetType() == typeof(Vector3))
            {
                vector3S[1] = (Vector3)args["position"];
            }
        }
        else
        {
            if (args.Contains("x"))
            {
                vector3S[1].x = (float)args["x"];
            }
            if (args.Contains("y"))
            {
                vector3S[1].y = (float)args["y"];
            }
            if (args.Contains("z"))
            {
                vector3S[1].z = (float)args["z"];
            }
        }

        //calculate:
        vector3S[3].x = Mathf.SmoothDamp(vector3S[0].x, vector3S[1].x, ref vector3S[2].x, time);
        vector3S[3].y = Mathf.SmoothDamp(vector3S[0].y, vector3S[1].y, ref vector3S[2].y, time);
        vector3S[3].z = Mathf.SmoothDamp(vector3S[0].z, vector3S[1].z, ref vector3S[2].z, time);

        //handle orient to path:
        if (args.Contains("orienttopath") && (bool)args["orienttopath"])
        {
            args["looktarget"] = vector3S[3];
        }

        //look applications:
        if (args.Contains("looktarget"))
        {
            iTween.LookUpdate(target, args);
        }

        //apply:
        if (isLocal)
        {
            target.transform.localPosition = vector3S[3];
        }
        else
        {
            target.transform.position = vector3S[3];
        }

        //need physics?
        if (target.GetComponent<Rigidbody>() != null)
        {
            Vector3 postUpdate = target.transform.position;
            target.transform.position = preUpdate;
            target.GetComponent<Rigidbody>().MovePosition(postUpdate);
        }
    }

    /// <summary>
    /// Similar to MoveTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with MINIMUM customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="position">
    /// A <see cref="Vector3"/> for a point in space the GameObject will animate to.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void MoveUpdate(GameObject target, Vector3 position, float time)
    {
        MoveUpdate(target, Hash("position", position, "time", time));
    }

    /// <summary>
    /// Similar to LookTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="looktarget">
    /// A <see cref="Transform"/> or <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="axis">
    /// A <see cref="System.String"/>. Restricts rotation to the supplied axis only.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> or <see cref="System.Double"/> for the time in seconds the animation will take to complete.
    /// </param> 
    public static void LookUpdate(GameObject target, Hashtable args)
    {
        CleanArgs(args);

        float time;
        Vector3[] vector3S = new Vector3[5];

        //set smooth time:
        if (args.Contains("looktime"))
        {
            time = (float)args["looktime"];
            time *= Defaults.UpdateTimePercentage;
        }
        else if (args.Contains("time"))
        {
            time = (float)args["time"] * .15f;
            time *= Defaults.UpdateTimePercentage;
        }
        else
        {
            time = Defaults.UpdateTime;
        }

        //from values:
        vector3S[0] = target.transform.eulerAngles;

        //set look:
        if (args.Contains("looktarget"))
        {
            if (args["looktarget"].GetType() == typeof(Transform))
            {
                //target.transform.LookAt((Transform)args["looktarget"]);
                target.transform.LookAt((Transform)args["looktarget"], (Vector3?)args["up"] ?? Defaults.Up);
            }
            else if (args["looktarget"].GetType() == typeof(Vector3))
            {
                //target.transform.LookAt((Vector3)args["looktarget"]);
                target.transform.LookAt((Vector3)args["looktarget"], (Vector3?)args["up"] ?? Defaults.Up);
            }
        }
        else
        {
            Debug.LogError("iTween Error: LookUpdate needs a 'looktarget' property!");
            return;
        }

        //to values and reset look:
        vector3S[1] = target.transform.eulerAngles;
        target.transform.eulerAngles = vector3S[0];

        //calculate:
        vector3S[3].x = Mathf.SmoothDampAngle(vector3S[0].x, vector3S[1].x, ref vector3S[2].x, time);
        vector3S[3].y = Mathf.SmoothDampAngle(vector3S[0].y, vector3S[1].y, ref vector3S[2].y, time);
        vector3S[3].z = Mathf.SmoothDampAngle(vector3S[0].z, vector3S[1].z, ref vector3S[2].z, time);

        //apply:
        target.transform.eulerAngles = vector3S[3];

        //axis restriction:
        if (args.Contains("axis"))
        {
            vector3S[4] = target.transform.eulerAngles;
            switch ((string)args["axis"])
            {
                case "x":
                    vector3S[4].y = vector3S[0].y;
                    vector3S[4].z = vector3S[0].z;
                    break;
                case "y":
                    vector3S[4].x = vector3S[0].x;
                    vector3S[4].z = vector3S[0].z;
                    break;
                case "z":
                    vector3S[4].x = vector3S[0].x;
                    vector3S[4].y = vector3S[0].y;
                    break;
            }

            //apply axis restriction:
            target.transform.eulerAngles = vector3S[4];
        }
    }

    /// <summary>
    /// Similar to LookTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a "live" set of changing values with FULL customization options. Does not utilize an EaseType. 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/> to be the target of the animation.
    /// </param>
    /// <param name="looktarget">
    /// A <see cref="Vector3"/> for a target the GameObject will look at.
    /// </param>
    /// <param name="time">
    /// A <see cref="System.Single"/> for the time in seconds the animation will take to complete.
    /// </param>
    public static void LookUpdate(GameObject target, Vector3 looktarget, float time)
    {
        LookUpdate(target, Hash("looktarget", looktarget, "time", time));
    }

    #endregion

    #region #7 External Utilities

    /// <summary>
    /// Returns the length of a curved path drawn through the provided array of Transforms.
    /// </summary>
    /// <returns>
    /// A <see cref="System.Single"/>
    /// </returns>
    /// <param name='path'>
    /// A <see cref="Transform[]"/>
    /// </param>
    public static float PathLength(Transform[] path)
    {
        Vector3[] suppliedPath = new Vector3[path.Length];
        float pathLength = 0;

        //create and store path points:
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }

        Vector3[] vector3S = PathControlPointGenerator(suppliedPath);

        //Line Draw:
        Vector3 prevPt = Interp(vector3S, 0);
        int smoothAmount = path.Length * 20;
        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currPt = Interp(vector3S, pm);
            pathLength += Vector3.Distance(prevPt, currPt);
            prevPt = currPt;
        }

        return pathLength;
    }

    /// <summary>
    /// Returns the length of a curved path drawn through the provided array of Vector3s.
    /// </summary>
    /// <returns>
    /// The length.
    /// </returns>
    /// <param name='path'>
    /// A <see cref="Vector3[]"/>
    /// </param>
    public static float PathLength(Vector3[] path)
    {
        float pathLength = 0;

        Vector3[] vector3S = PathControlPointGenerator(path);

        //Line Draw:
        Vector3 prevPt = Interp(vector3S, 0);
        int smoothAmount = path.Length * 20;
        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currPt = Interp(vector3S, pm);
            pathLength += Vector3.Distance(prevPt, currPt);
            prevPt = currPt;
        }

        return pathLength;
    }

    /// <summary>
    /// Creates and returns a full-screen Texture2D for use with CameraFade.
    /// </summary>
    /// <returns>
    /// Texture2D
    /// </returns>
    /// <param name='color'>
    /// Color
    /// </param>
    public static Texture2D CameraTexture(Color color)
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        Color[] colors = new Color[Screen.width * Screen.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        texture.SetPixels(colors);
        texture.Apply();
        return (texture);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Vector3[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(GameObject target, Vector3[] path, float percent)
    {
        target.transform.position = Interp(PathControlPointGenerator(path), percent);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="Transform"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Vector3[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(Transform target, Vector3[] path, float percent)
    {
        target.position = Interp(PathControlPointGenerator(path), percent);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="GameObject"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(GameObject target, Transform[] path, float percent)
    {
        //create and store path points:
        Vector3[] suppliedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }
        target.transform.position = Interp(PathControlPointGenerator(suppliedPath), percent);
    }

    /// <summary>
    /// Puts a GameObject on a path at the provided percentage 
    /// </summary>
    /// <param name="target">
    /// A <see cref="Transform"/>
    /// </param>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    public static void PutOnPath(Transform target, Transform[] path, float percent)
    {
        //create and store path points:
        Vector3[] suppliedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }
        target.position = Interp(PathControlPointGenerator(suppliedPath), percent);
    }

    /// <summary>
    /// Returns a Vector3 position on a path at the provided percentage  
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    /// <returns>
    /// A <see cref="Vector3"/>
    /// </returns>
    public static Vector3 PointOnPath(Transform[] path, float percent)
    {
        //create and store path points:
        Vector3[] suppliedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            suppliedPath[i] = path[i].position;
        }
        return (Interp(PathControlPointGenerator(suppliedPath), percent));
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Vector3s.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawLine(Vector3[] line)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Vector3s.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLine(Vector3[] line, Color color)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Transforms.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawLine(Transform[] line)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }
            DrawLineHelper(suppliedLine, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a line through the provided array of Transforms.
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLine(Transform[] line, Color color)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }

            DrawLineHelper(suppliedLine, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawLineGizmos(Vector3[] line)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineGizmos(Vector3[] line, Color color)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawLineGizmos(Transform[] line)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }
            DrawLineHelper(suppliedLine, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineGizmos(Transform[] line, Color color)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }

            DrawLineHelper(suppliedLine, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawLineHandles(Vector3[] line)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, Defaults.Color, "handles");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineHandles(Vector3[] line, Color color)
    {
        if (line.Length > 0)
        {
            DrawLineHelper(line, color, "handles");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawLineHandles(Transform[] line)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }
            DrawLineHelper(suppliedLine, Defaults.Color, "handles");
        }
    }

    /// <summary>
    /// Draws a line through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="line">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawLineHandles(Transform[] line, Color color)
    {
        if (line.Length > 0)
        {
            //create and store line points:
            Vector3[] suppliedLine = new Vector3[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                suppliedLine[i] = line[i].position;
            }

            DrawLineHelper(suppliedLine, color, "handles");
        }
    }

    /// <summary>
    /// Returns a Vector3 position on a path at the provided percentage  
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3[]"/>
    /// </param>
    /// <param name="percent">
    /// A <see cref="System.Single"/>
    /// </param>
    /// <returns>
    /// A <see cref="Vector3"/>
    /// </returns>
    public static Vector3 PointOnPath(Vector3[] path, float percent)
    {
        return (Interp(PathControlPointGenerator(path), percent));
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Vector3s.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawPath(Vector3[] path)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Vector3s.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPath(Vector3[] path, Color color)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Transforms.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawPath(Transform[] path)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// When called from an OnDrawGizmos() function it will draw a curved path through the provided array of Transforms.
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPath(Transform[] path, Color color)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawPathGizmos(Vector3[] path)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathGizmos(Vector3[] path, Color color)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawPathGizmos(Transform[] path)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, Defaults.Color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Gizmos.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathGizmos(Transform[] path, Color color)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, color, "gizmos");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    public static void DrawPathHandles(Vector3[] path)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, Defaults.Color, "handles");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Vector3s with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Vector3s[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathHandles(Vector3[] path, Color color)
    {
        if (path.Length > 0)
        {
            DrawPathHelper(path, color, "handles");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    public static void DrawPathHandles(Transform[] path)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, Defaults.Color, "handles");
        }
    }

    /// <summary>
    /// Draws a curved path through the provided array of Transforms with Handles.DrawLine().
    /// </summary>
    /// <param name="path">
    /// A <see cref="Transform[]"/>
    /// </param>
    /// <param name="color">
    /// A <see cref="Color"/>
    /// </param> 
    public static void DrawPathHandles(Transform[] path, Color color)
    {
        if (path.Length > 0)
        {
            //create and store path points:
            Vector3[] suppliedPath = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                suppliedPath[i] = path[i].position;
            }

            DrawPathHelper(suppliedPath, color, "handles");
        }
    }

    /// <summary>
    /// Changes a camera fade's texture.
    /// </summary>
    /// <param name="depth">
    /// A <see cref="System.Int32"/>
    /// </param>
    public static void CameraFadeDepth(int depth)
    {
        if (_cameraFade)
        {
            _cameraFade.transform.position = new Vector3(_cameraFade.transform.position.x, _cameraFade.transform.position.y, depth);
        }
    }

    /// <summary>
    /// Removes and destroyes a camera fade.
    /// </summary>
    public static void CameraFadeDestroy()
    {
        if (_cameraFade)
        {
            Destroy(_cameraFade);
        }
    }

    /// <summary>
    /// Changes a camera fade's texture.
    /// </summary>
    /// <param name='texture'>
    /// A <see cref="Texture2D"/>
    /// </param>
    public static void CameraFadeSwap(Texture2D texture)
    {
        if (_cameraFade)
        {
            _cameraFade.GetComponent<GUITexture>().texture = texture;
        }
    }

    /// <summary>
    /// Creates a GameObject (if it doesn't exist) at the supplied depth that can be used to simulate a camera fade.
    /// </summary>
    /// <param name='texture'>
    /// A <see cref="Texture2D"/>
    /// </param>
    /// <param name='depth'>
    /// A <see cref="System.Int32"/>
    /// </param>
    /// <returns>
    /// A <see cref="GameObject"/> for a reference to the CameraFade.
    /// </returns>
    public static GameObject CameraFadeAdd(Texture2D texture, int depth)
    {
        if (_cameraFade)
        {
            return null;
        }
        else
        {
            //establish colorFade object:
            _cameraFade = new GameObject("iTween Camera Fade");
            _cameraFade.transform.position = new Vector3(.5f, .5f, depth);
            _cameraFade.AddComponent<GUITexture>();
            _cameraFade.GetComponent<GUITexture>().texture = texture;
            _cameraFade.GetComponent<GUITexture>().color = new Color(.5f, .5f, .5f, 0);
            return _cameraFade;
        }
    }

    /// <summary>
    /// Creates a GameObject (if it doesn't exist) at the default depth that can be used to simulate a camera fade.
    /// </summary>
    /// <param name='texture'>
    /// A <see cref="Texture2D"/>
    /// </param>
    /// <returns>
    /// A <see cref="GameObject"/> for a reference to the CameraFade.
    /// </returns>
    public static GameObject CameraFadeAdd(Texture2D texture)
    {
        if (_cameraFade)
        {
            return null;
        }
        else
        {
            //establish colorFade object:
            _cameraFade = new GameObject("iTween Camera Fade");
            _cameraFade.transform.position = new Vector3(.5f, .5f, Defaults.cameraFadeDepth);
            _cameraFade.AddComponent<GUITexture>();
            _cameraFade.GetComponent<GUITexture>().texture = texture;
            _cameraFade.GetComponent<GUITexture>().color = new Color(.5f, .5f, .5f, 0);
            return _cameraFade;
        }
    }

    /// <summary>
    /// Creates a GameObject (if it doesn't exist) at the default depth filled with black that can be used to simulate a camera fade.
    /// </summary>
    /// <returns>
    /// A <see cref="GameObject"/> for a reference to the CameraFade.
    /// </returns>
    public static GameObject CameraFadeAdd()
    {
        if (_cameraFade)
        {
            return null;
        }
        else
        {
            //establish colorFade object:
            _cameraFade = new GameObject("iTween Camera Fade");
            _cameraFade.transform.position = new Vector3(.5f, .5f, Defaults.cameraFadeDepth);
            _cameraFade.AddComponent<GUITexture>();
            _cameraFade.GetComponent<GUITexture>().texture = CameraTexture(Color.black);
            _cameraFade.GetComponent<GUITexture>().color = new Color(.5f, .5f, .5f, 0);
            return _cameraFade;
        }
    }


    //#################################
    //# RESUME UTILITIES AND OVERLOADS # 
    //#################################	

    /// <summary>
    /// Resume all iTweens on a GameObject.
    /// </summary>
    public static void Resume(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            item.enabled = true;
        }
    }

    /// <summary>
    /// Resume all iTweens on a GameObject including its children.
    /// </summary>
    public static void Resume(GameObject target, bool includechildren)
    {
        Resume(target);
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Resume(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Resume all iTweens on a GameObject of a particular type.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Resume(GameObject target, string type)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.enabled = true;
            }
        }
    }

    /// <summary>
    /// Resume all iTweens on a GameObject of a particular type including its children.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Resume(GameObject target, string type, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.enabled = true;
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Resume(child.gameObject, type, true);
            }
        }
    }

    /// <summary>
    /// Resume all iTweens in scene.
    /// </summary>
    public static void Resume()
    {
        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            Resume(target);
        }
    }

    /// <summary>
    /// Resume all iTweens in scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to resume.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static void Resume(string type)
    {
        ArrayList resumeArray = new ArrayList();

        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            resumeArray.Insert(resumeArray.Count, target);
        }

        for (int i = 0; i < resumeArray.Count; i++)
        {
            Resume((GameObject)resumeArray[i], type);
        }
    }

    //#################################
    //# PAUSE UTILITIES AND OVERLOADS # 
    //#################################

    /// <summary>
    /// Pause all iTweens on a GameObject.
    /// </summary>
    public static void Pause(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            if (item.Delay > 0)
            {
                item.Delay -= UnityEngine.Time.time - item._delayStarted;
                item.StopCoroutine("TweenDelay");
            }
            item.IsPaused = true;
            item.enabled = false;
        }
    }

    /// <summary>
    /// Pause all iTweens on a GameObject including its children.
    /// </summary>
    public static void Pause(GameObject target, bool includechildren)
    {
        Pause(target);
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Pause(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Pause all iTweens on a GameObject of a particular type.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Pause(GameObject target, string type)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                if (item.Delay > 0)
                {
                    item.Delay -= UnityEngine.Time.time - item._delayStarted;
                    item.StopCoroutine("TweenDelay");
                }
                item.IsPaused = true;
                item.enabled = false;
            }
        }
    }

    /// <summary>
    /// Pause all iTweens on a GameObject of a particular type including its children.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Pause(GameObject target, string type, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                if (item.Delay > 0)
                {
                    item.Delay -= UnityEngine.Time.time - item._delayStarted;
                    item.StopCoroutine("TweenDelay");
                }
                item.IsPaused = true;
                item.enabled = false;
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Pause(child.gameObject, type, true);
            }
        }
    }

    /// <summary>
    /// Pause all iTweens in scene.
    /// </summary>
    public static void Pause()
    {
        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            Pause(target);
        }
    }

    /// <summary>
    /// Pause all iTweens in scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to pause.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static void Pause(string type)
    {
        ArrayList pauseArray = new ArrayList();

        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            pauseArray.Insert(pauseArray.Count, target);
        }

        for (int i = 0; i < pauseArray.Count; i++)
        {
            Pause((GameObject)pauseArray[i], type);
        }
    }

    //#################################
    //# COUNT UTILITIES AND OVERLOADS # 
    //#################################	

    /// <summary>
    /// Count all iTweens in current scene.
    /// </summary>
    public static int Count()
    {
        return (Tweens.Count);
    }

    /// <summary>
    /// Count all iTweens in current scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static int Count(string type)
    {
        int tweenCount = 0;

        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            string targetType = (string)currentTween["type"] + (string)currentTween["method"];
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                tweenCount++;
            }
        }

        return (tweenCount);
    }

    /// <summary>
    /// Count all iTweens on a GameObject.
    /// </summary>
    public static int Count(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        return (tweens.Length);
    }

    /// <summary>
    /// Count all iTweens on a GameObject of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to count.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>  
    public static int Count(GameObject target, string type)
    {
        int tweenCount = 0;
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                tweenCount++;
            }
        }
        return (tweenCount);
    }

    //################################
    //# STOP UTILITIES AND OVERLOADS # 
    //################################	

    /// <summary>
    /// Stop and destroy all Tweens in current scene.
    /// </summary>
    public static void Stop()
    {
        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            Stop(target);
        }
        Tweens.Clear();
    }

    /// <summary>
    /// Stop and destroy all iTweens in current scene of a particular type.
    /// </summary>
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param> 
    public static void Stop(string type)
    {
        ArrayList stopArray = new ArrayList();

        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            stopArray.Insert(stopArray.Count, target);
        }

        for (int i = 0; i < stopArray.Count; i++)
        {
            Stop((GameObject)stopArray[i], type);
        }
    }

    /* GFX47 MOD START */
    /// <summary>
    /// Stop and destroy all iTweens in current scene of a particular name.
    /// </summary>
    /// <param name="name">
    /// The <see cref="System.String"/> name of iTween you would like to stop.
    /// </param> 
    public static void StopByName(string name)
    {
        ArrayList stopArray = new ArrayList();

        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable currentTween = Tweens[i];
            GameObject target = (GameObject)currentTween["target"];
            stopArray.Insert(stopArray.Count, target);
        }

        for (int i = 0; i < stopArray.Count; i++)
        {
            StopByName((GameObject)stopArray[i], name);
        }
    }
    /* GFX47 MOD END */

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject.
    /// </summary>
    public static void Stop(GameObject target)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            item.Dispose();
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject including its children.
    /// </summary>
    public static void Stop(GameObject target, bool includechildren)
    {
        Stop(target);
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Stop(child.gameObject, true);
            }
        }
    }

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular type.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Stop(GameObject target, string type)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.Dispose();
            }
        }
    }

    /* GFX47 MOD START */
    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular name.
    /// </summar
    /// <param name="name">
    /// The <see cref="System.String"/> name of iTween you would like to stop.
    /// </param>	
    public static void StopByName(GameObject target, string name)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            /*string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.Dispose();
			}*/
            if (item.Name == name)
            {
                item.Dispose();
            }
        }
    }
    /* GFX47 MOD END */

    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular type including its children.
    /// </summar
    /// <param name="type">
    /// A <see cref="System.String"/> name of the type of iTween you would like to stop.  Can be written as part of a name such as "mov" for all "MoveTo" iTweens.
    /// </param>	
    public static void Stop(GameObject target, string type, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            string targetType = item.Type + item.Method;
            targetType = targetType.Substring(0, type.Length);
            if (targetType.ToLower() == type.ToLower())
            {
                item.Dispose();
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                Stop(child.gameObject, type, true);
            }
        }
    }

    /* GFX47 MOD START */
    /// <summary>
    /// Stop and destroy all iTweens on a GameObject of a particular name including its children.
    /// </summar
    /// <param name="name">
    /// The <see cref="System.String"/> name of iTween you would like to stop.
    /// </param>	
    public static void StopByName(GameObject target, string name, bool includechildren)
    {
        Component[] tweens = target.GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            /*string targetType = item.type+item.method;
			targetType=targetType.Substring(0,type.Length);
			if(targetType.ToLower() == type.ToLower()){
				item.Dispose();
			}*/
            if (item.Name == name)
            {
                item.Dispose();
            }
        }
        if (includechildren)
        {
            foreach (Transform child in target.transform)
            {
                //Stop(child.gameObject,type,true);
                StopByName(child.gameObject, name, true);
            }
        }
    }
    /* GFX47 MOD END */

    /// <summary>
    /// Universal interface to help in the creation of Hashtables.  Especially useful for C# users.
    /// </summary>
    /// <param name="args">
    /// A <see cref="System.Object[]"/> of alternating name value pairs.  For example "time",1,"delay",2...
    /// </param>
    /// <returns>
    /// A <see cref="Hashtable"/>
    /// </returns>
    public static Hashtable Hash(params object[] args)
    {
        Hashtable hashTable = new Hashtable(args.Length / 2);
        if (args.Length % 2 != 0)
        {
            Debug.LogError("Tween Error: Hash requires an even number of arguments!");
            return null;
        }
        else
        {
            int i = 0;
            while (i < args.Length - 1)
            {
                hashTable.Add(args[i], args[i + 1]);
                i += 2;
            }
            return hashTable;
        }
    }

    #endregion

    #region Component Segments

    private iTween(Hashtable h)
    {
        _tweenArguments = h;
    }

    void Awake()
    {
        _thisTransform = transform;

        RetrieveArgs();
        _lastRealTime = UnityEngine.Time.realtimeSinceStartup; // Added by PressPlay
    }

    IEnumerator Start()
    {
        if (Delay > 0)
        {
            yield return StartCoroutine("TweenDelay");
        }
        TweenStart();
    }

    //non-physics
    void Update()
    {
        if (IsRunning && !_physics)
        {
            if (!_reverse)
            {
                if (_percentage < 1f)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
            else
            {
                if (_percentage > 0)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
        }
    }

    //physics
    void FixedUpdate()
    {
        if (IsRunning && _physics)
        {
            if (!_reverse)
            {
                if (_percentage < 1f)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
            else
            {
                if (_percentage > 0)
                {
                    TweenUpdate();
                }
                else
                {
                    TweenComplete();
                }
            }
        }
    }

    void LateUpdate()
    {
        //look applications:
        if (_tweenArguments.Contains("looktarget") && IsRunning)
        {
            if (Type == "move" || Type == "shake" || Type == "punch")
            {
                LookUpdate(gameObject, _tweenArguments);
            }
        }
    }

    void OnEnable()
    {
        if (IsRunning)
        {
            EnableKinematic();
        }

        //resume delay:
        if (IsPaused)
        {
            IsPaused = false;
            if (Delay > 0)
            {
                _wasPaused = true;
                ResumeDelay();
            }
        }
    }

    void OnDisable()
    {
        DisableKinematic();
    }

    #endregion

    #region Internal Helpers

    private static void DrawLineHelper(Vector3[] line, Color color, string method)
    {
        Gizmos.color = color;
        for (int i = 0; i < line.Length - 1; i++)
        {
            if (method == "gizmos")
            {
                Gizmos.DrawLine(line[i], line[i + 1]); ;
            }
            else if (method == "handles")
            {
                Debug.LogError("iTween Error: Drawing a line with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
                //UnityEditor.Handles.DrawLine(line[i], line[i+1]);
            }
        }
    }

    private static void DrawPathHelper(Vector3[] path, Color color, string method)
    {
        Vector3[] vector3S = PathControlPointGenerator(path);

        //Line Draw:
        Vector3 prevPt = Interp(vector3S, 0);
        Gizmos.color = color;
        int smoothAmount = path.Length * 20;
        for (int i = 1; i <= smoothAmount; i++)
        {
            float pm = (float)i / smoothAmount;
            Vector3 currPt = Interp(vector3S, pm);
            if (method == "gizmos")
            {
                Gizmos.DrawLine(currPt, prevPt);
            }
            else if (method == "handles")
            {
                Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
                //UnityEditor.Handles.DrawLine(currPt, prevPt);
            }
            prevPt = currPt;
        }
    }

    private static Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3S;

        //create and store path points:
        suppliedPath = path;

        //populate calculate path;
        int offset = 2;
        vector3S = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3S, 1, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        vector3S[0] = vector3S[1] + (vector3S[1] - vector3S[2]);
        vector3S[vector3S.Length - 1] = vector3S[vector3S.Length - 2] + (vector3S[vector3S.Length - 2] - vector3S[vector3S.Length - 3]);

        //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
        if (vector3S[1] == vector3S[vector3S.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3S.Length];
            Array.Copy(vector3S, tmpLoopSpline, vector3S.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3S = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3S, tmpLoopSpline.Length);
        }

        return (vector3S);
    }

    //andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
    private static Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    //andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
    private class CrSpline
    {
        public Vector3[] Pts;

        public CrSpline(params Vector3[] pts)
        {
            this.Pts = new Vector3[pts.Length];
            Array.Copy(pts, this.Pts, pts.Length);
        }


        public Vector3 Interp(float t)
        {
            int numSections = Pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;
            Vector3 a = Pts[currPt];
            Vector3 b = Pts[currPt + 1];
            Vector3 c = Pts[currPt + 2];
            Vector3 d = Pts[currPt + 3];
            return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
        }
    }

    //catalog new tween and add component phase of iTween:
    static void Launch(GameObject target, Hashtable args)
    {
        if (!args.Contains("id"))
        {
            args["id"] = GenerateId();
        }
        if (!args.Contains("target"))
        {
            args["target"] = target;

        }

        Tweens.Insert(0, args);
        target.AddComponent<iTween>();
    }

    //cast any accidentally supplied doubles and ints as floats as iTween only uses floats internally and unify parameter case:
    static Hashtable CleanArgs(Hashtable args)
    {
        Hashtable argsCopy = new Hashtable(args.Count);
        Hashtable argsCaseUnified = new Hashtable(args.Count);

        foreach (DictionaryEntry item in args)
        {
            argsCopy.Add(item.Key, item.Value);
        }

        foreach (DictionaryEntry item in argsCopy)
        {
            if (item.Value.GetType() == typeof(System.Int32))
            {
                int original = (int)item.Value;
                float casted = (float)original;
                args[item.Key] = casted;
            }
            if (item.Value.GetType() == typeof(System.Double))
            {
                double original = (double)item.Value;
                float casted = (float)original;
                args[item.Key] = casted;
            }
        }

        //unify parameter case:
        foreach (DictionaryEntry item in args)
        {
            argsCaseUnified.Add(item.Key.ToString().ToLower(), item.Value);
        }

        //swap back case unification:
        args = argsCaseUnified;

        return args;
    }

    //random ID generator:
    static string GenerateId()
    {
        //		int strlen = 15;
        //		char[] chars = {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','0','1','2','3','4','5','6','7','8'};
        //		int num_chars = chars.Length - 1;
        //		string randomChar = "";
        //		for (int i = 0; i < strlen; i++) {
        //			randomChar += chars[(int)Mathf.Floor(UnityEngine.Random.Range(0,num_chars))];
        //		}
        return System.Guid.NewGuid().ToString();
    }

    //grab and set generic, neccesary iTween arguments:
    void RetrieveArgs()
    {
        foreach (Hashtable item in Tweens)
        {
            if ((GameObject)item["target"] == gameObject)
            {
                _tweenArguments = item;
                break;
            }
        }

        Id = (string)_tweenArguments["id"];
        Type = (string)_tweenArguments["type"];
        /* GFX47 MOD START */
        Name = (string)_tweenArguments["name"];
        /* GFX47 MOD END */
        Method = (string)_tweenArguments["method"];

        if (_tweenArguments.Contains("time"))
        {
            Time = (float)_tweenArguments["time"];
        }
        else
        {
            Time = Defaults.Time;
        }

        //do we need to use physics, is there a rigidbody?
        if (GetComponent<Rigidbody>() != null)
        {
            _physics = true;
        }

        if (_tweenArguments.Contains("delay"))
        {
            Delay = (float)_tweenArguments["delay"];
        }
        else
        {
            Delay = Defaults.Delay;
        }

        if (_tweenArguments.Contains("namedcolorvalue"))
        {
            //allows namedcolorvalue to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (_tweenArguments["namedcolorvalue"].GetType() == typeof(NamedValueColor))
            {
                _namedcolorvalue = (NamedValueColor)_tweenArguments["namedcolorvalue"];
            }
            else
            {
                try
                {
                    _namedcolorvalue = (NamedValueColor)Enum.Parse(typeof(NamedValueColor), (string)_tweenArguments["namedcolorvalue"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported namedcolorvalue supplied! Default will be used.");
                    _namedcolorvalue = iTween.NamedValueColor.Color;
                }
            }
        }
        else
        {
            _namedcolorvalue = Defaults.NamedColorValue;
        }

        if (_tweenArguments.Contains("looptype"))
        {
            //allows loopType to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (_tweenArguments["looptype"].GetType() == typeof(LoopType))
            {
                loopType = (LoopType)_tweenArguments["looptype"];
            }
            else
            {
                try
                {
                    loopType = (LoopType)Enum.Parse(typeof(LoopType), (string)_tweenArguments["looptype"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported loopType supplied! Default will be used.");
                    loopType = iTween.LoopType.None;
                }
            }
        }
        else
        {
            loopType = iTween.LoopType.None;
        }

        if (_tweenArguments.Contains("easetype"))
        {
            //allows easeType to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (_tweenArguments["easetype"].GetType() == typeof(EaseType))
            {
                easeType = (EaseType)_tweenArguments["easetype"];
            }
            else
            {
                try
                {
                    easeType = (EaseType)Enum.Parse(typeof(EaseType), (string)_tweenArguments["easetype"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported easeType supplied! Default will be used.");
                    easeType = Defaults.EaseType;
                }
            }
        }
        else
        {
            easeType = Defaults.EaseType;
        }

        if (_tweenArguments.Contains("space"))
        {
            //allows space to be set as either an enum(C# friendly) or a string(JS friendly), string case usage doesn't matter to further increase usability:
            if (_tweenArguments["space"].GetType() == typeof(Space))
            {
                _space = (Space)_tweenArguments["space"];
            }
            else
            {
                try
                {
                    _space = (Space)Enum.Parse(typeof(Space), (string)_tweenArguments["space"], true);
                }
                catch
                {
                    Debug.LogWarning("iTween: Unsupported space supplied! Default will be used.");
                    _space = Defaults.Space;
                }
            }
        }
        else
        {
            _space = Defaults.Space;
        }

        if (_tweenArguments.Contains("islocal"))
        {
            _isLocal = (bool)_tweenArguments["islocal"];
        }
        else
        {
            _isLocal = Defaults.IsLocal;
        }

        // Added by PressPlay
        if (_tweenArguments.Contains("ignoretimescale"))
        {
            _useRealTime = (bool)_tweenArguments["ignoretimescale"];
        }
        else
        {
            _useRealTime = Defaults.UseRealTime;
        }

        //instantiates a cached ease equation reference:
        GetEasingFunction();
    }

    //instantiates a cached ease equation refrence:
    void GetEasingFunction()
    {
        switch (easeType)
        {
            case EaseType.EaseInQuad:
                _ease = new EasingFunction(EaseInQuad);
                break;
            case EaseType.EaseOutQuad:
                _ease = new EasingFunction(EaseOutQuad);
                break;
            case EaseType.EaseInOutQuad:
                _ease = new EasingFunction(EaseInOutQuad);
                break;
            case EaseType.EaseInCubic:
                _ease = new EasingFunction(EaseInCubic);
                break;
            case EaseType.EaseOutCubic:
                _ease = new EasingFunction(EaseOutCubic);
                break;
            case EaseType.EaseInOutCubic:
                _ease = new EasingFunction(EaseInOutCubic);
                break;
            case EaseType.EaseInQuart:
                _ease = new EasingFunction(EaseInQuart);
                break;
            case EaseType.EaseOutQuart:
                _ease = new EasingFunction(EaseOutQuart);
                break;
            case EaseType.EaseInOutQuart:
                _ease = new EasingFunction(EaseInOutQuart);
                break;
            case EaseType.EaseInQuint:
                _ease = new EasingFunction(EaseInQuint);
                break;
            case EaseType.EaseOutQuint:
                _ease = new EasingFunction(EaseOutQuint);
                break;
            case EaseType.EaseInOutQuint:
                _ease = new EasingFunction(EaseInOutQuint);
                break;
            case EaseType.EaseInSine:
                _ease = new EasingFunction(EaseInSine);
                break;
            case EaseType.EaseOutSine:
                _ease = new EasingFunction(EaseOutSine);
                break;
            case EaseType.EaseInOutSine:
                _ease = new EasingFunction(EaseInOutSine);
                break;
            case EaseType.EaseInExpo:
                _ease = new EasingFunction(EaseInExpo);
                break;
            case EaseType.EaseOutExpo:
                _ease = new EasingFunction(EaseOutExpo);
                break;
            case EaseType.EaseInOutExpo:
                _ease = new EasingFunction(EaseInOutExpo);
                break;
            case EaseType.EaseInCirc:
                _ease = new EasingFunction(EaseInCirc);
                break;
            case EaseType.EaseOutCirc:
                _ease = new EasingFunction(EaseOutCirc);
                break;
            case EaseType.EaseInOutCirc:
                _ease = new EasingFunction(EaseInOutCirc);
                break;
            case EaseType.Linear:
                _ease = new EasingFunction(Linear);
                break;
            case EaseType.Spring:
                _ease = new EasingFunction(Spring);
                break;
            /* GFX47 MOD START */
            /*case EaseType.bounce:
                ease = new EasingFunction(bounce);
                break;*/
            case EaseType.EaseInBounce:
                _ease = new EasingFunction(EaseInBounce);
                break;
            case EaseType.EaseOutBounce:
                _ease = new EasingFunction(EaseOutBounce);
                break;
            case EaseType.EaseInOutBounce:
                _ease = new EasingFunction(EaseInOutBounce);
                break;
            /* GFX47 MOD END */
            case EaseType.EaseInBack:
                _ease = new EasingFunction(EaseInBack);
                break;
            case EaseType.EaseOutBack:
                _ease = new EasingFunction(EaseOutBack);
                break;
            case EaseType.EaseInOutBack:
                _ease = new EasingFunction(EaseInOutBack);
                break;
            /* GFX47 MOD START */
            /*case EaseType.elastic:
                ease = new EasingFunction(elastic);
                break;*/
            case EaseType.EaseInElastic:
                _ease = new EasingFunction(EaseInElastic);
                break;
            case EaseType.EaseOutElastic:
                _ease = new EasingFunction(EaseOutElastic);
                break;
            case EaseType.EaseInOutElastic:
                _ease = new EasingFunction(EaseInOutElastic);
                break;
                /* GFX47 MOD END */
        }
    }

    //calculate percentage of tween based on time:
    void UpdatePercentage()
    {

        // Added by PressPlay   
        if (_useRealTime)
        {
            _runningTime += (UnityEngine.Time.realtimeSinceStartup - _lastRealTime);
        }
        else
        {
            _runningTime += UnityEngine.Time.deltaTime;
        }

        if (_reverse)
        {
            _percentage = 1 - _runningTime / Time;
        }
        else
        {
            _percentage = _runningTime / Time;
        }

        _lastRealTime = UnityEngine.Time.realtimeSinceStartup; // Added by PressPlay
    }

    void CallBack(string callbackType)
    {
        if (_tweenArguments.Contains(callbackType) && !_tweenArguments.Contains("ischild"))
        {
            //establish target:
            GameObject target;
            if (_tweenArguments.Contains(callbackType + "target"))
            {
                target = (GameObject)_tweenArguments[callbackType + "target"];
            }
            else
            {
                target = gameObject;
            }

            //Check if the callback type is a string or a delegate
            if (_tweenArguments[callbackType] is string)
            {
                target.SendMessage((string)_tweenArguments[callbackType], (object)_tweenArguments[callbackType + "params"], SendMessageOptions.DontRequireReceiver);
            }
            else if (_tweenArguments[callbackType] is Action)
            {
                var callbackAction = (Action)_tweenArguments[callbackType];
                callbackAction();
            }
            else
            {
                Debug.LogError("iTween Error: Callback method references must be passed as a String! (or action! --Fano)");
                Destroy(this);
            }
        }
    }

    void Dispose()
    {
        for (int i = 0; i < Tweens.Count; i++)
        {
            Hashtable tweenEntry = Tweens[i];
            if ((string)tweenEntry["id"] == Id)
            {
                Tweens.RemoveAt(i);
                break;
            }
        }
        Destroy(this);
    }

    void ConflictCheck()
    {//if a new iTween is about to run and is of the same type as an in progress iTween this will destroy the previous if the new one is NOT identical in every way or it will destroy the new iTween if they are:	
        Component[] tweens = GetComponents<iTween>();
        foreach (iTween item in tweens)
        {
            if (item.Type == "value")
            {
                return;
            }
            else if (item.IsRunning && item.Type == Type)
            {
                //cancel out if this is a shake or punch variant:
                if (item.Method != Method)
                {
                    return;
                }

                //step 1: check for length first since it's the fastest:
                if (item._tweenArguments.Count != _tweenArguments.Count)
                {
                    item.Dispose();
                    return;
                }

                //step 2: side-by-side check to figure out if this is an identical tween scenario to handle Update usages of iTween:
                foreach (DictionaryEntry currentProp in _tweenArguments)
                {
                    if (!item._tweenArguments.Contains(currentProp.Key))
                    {
                        item.Dispose();
                        return;
                    }
                    else
                    {
                        if (!item._tweenArguments[currentProp.Key].Equals(_tweenArguments[currentProp.Key]) && (string)currentProp.Key != "id")
                        {//if we aren't comparing ids and something isn't exactly the same replace the running iTween: 
                            item.Dispose();
                            return;
                        }
                    }
                }

                //step 3: prevent a new iTween addition if it is identical to the currently running iTween
                Dispose();
                //Destroy(this);	
            }
        }
    }

    void EnableKinematic()
    {
        /*
		if(gameObject.GetComponent(typeof(Rigidbody))){
			if(!rigidbody.isKinematic){
				kinematic=true;
				rigidbody.isKinematic=true;
			}
		}
		*/
    }

    void DisableKinematic()
    {
        /*
		if(kinematic && rigidbody.isKinematic==true){
			kinematic=false;
			rigidbody.isKinematic=false;
		}
		*/
    }

    void ResumeDelay()
    {
        StartCoroutine("TweenDelay");
    }

    #endregion

    #region Easing Curves

    private float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    private float Clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) * 0.5f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    private float Spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    private float EaseInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    private float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    private float EaseInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value + start;
        value--;
        return -end * 0.5f * (value * (value - 2) - 1) + start;
    }

    private float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    private float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    private float EaseInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value + start;
        value -= 2;
        return end * 0.5f * (value * value * value + 2) + start;
    }

    private float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    private float EaseOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    private float EaseInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value + start;
        value -= 2;
        return -end * 0.5f * (value * value * value * value - 2) + start;
    }

    private float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    private float EaseOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    private float EaseInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value * value + start;
        value -= 2;
        return end * 0.5f * (value * value * value * value * value + 2) + start;
    }

    private float EaseInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
    }

    private float EaseOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
    }

    private float EaseInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
    }

    private float EaseInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value - 1)) + start;
    }

    private float EaseOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
    }

    private float EaseInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    private float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    private float EaseOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt(1 - value * value) + start;
    }

    private float EaseInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    /* GFX47 MOD START */
    private float EaseInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - EaseOutBounce(0, end, d - value) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //private float bounce(float start, float end, float value){
    private float EaseOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    private float EaseInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d * 0.5f) return EaseInBounce(0, end, value * 2) * 0.5f + start;
        else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
    }
    /* GFX47 MOD END */

    private float EaseInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    private float EaseOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = (value) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    private float EaseInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }

    private float Punch(float amplitude, float value)
    {
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        else if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

    /* GFX47 MOD START */
    private float EaseInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //private float elastic(float start, float end, float value){
    private float EaseOutElastic(float start, float end, float value)
    {
        /* GFX47 MOD END */
        //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p * 0.25f;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }

    /* GFX47 MOD START */
    private float EaseInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d * 0.5f) == 2) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }
    /* GFX47 MOD END */

    #endregion

    #region Deprecated and Renamed
    /*
	public static void audioFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: audioFrom() has been renamed to AudioFrom().");}
	public static void audioTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: audioTo() has been renamed to AudioTo().");}
	public static void colorFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: colorFrom() has been renamed to ColorFrom().");}
	public static void colorTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: colorTo() has been renamed to ColorTo().");}
	public static void fadeFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: fadeFrom() has been renamed to FadeFrom().");}
	public static void fadeTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: fadeTo() has been renamed to FadeTo().");}
	public static void lookFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: lookFrom() has been renamed to LookFrom().");}
	public static void lookFromWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: lookFromWorld() has been deprecated. Please investigate LookFrom().");}
	public static void lookTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: lookTo() has been renamed to LookTo().");}
	public static void lookToUpdate(GameObject target, Hashtable args){Debug.LogError("iTween Error: lookToUpdate() has been renamed to LookUpdate().");}
	public static void lookToUpdateWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: lookToUpdateWorld() has been deprecated. Please investigate LookUpdate().");}
	public static void moveAdd(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveAdd() has been renamed to MoveAdd().");}
	public static void moveAddWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveAddWorld() has been deprecated. Please investigate MoveAdd().");}
	public static void moveBy(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveBy() has been renamed to MoveBy().");}
	public static void moveByWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveAddWorld() has been deprecated. Please investigate MoveAdd().");}
	public static void moveFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveFrom() has been renamed to MoveFrom().");}
	public static void moveFromWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveFromWorld() has been deprecated. Please investigate MoveFrom().");}
	public static void moveTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveTo() has been renamed to MoveTo().");}
	public static void moveToBezier(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveToBezier() has been deprecated. Please investigate MoveTo() and the "path" property.");}
	public static void moveToBezierWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveToBezierWorld() has been deprecated. Please investigate MoveTo() and the "path" property.");}
	public static void moveToUpdate(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveToUpdate() has been renamed to MoveUpdate().");}
	public static void moveToUpdateWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveToUpdateWorld() has been deprecated. Please investigate MoveUpdate().");}
	public static void moveToWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: moveToWorld() has been deprecated. Please investigate MoveTo().");}
	public static void punchPosition(GameObject target, Hashtable args){Debug.LogError("iTween Error: punchPosition() has been renamed to PunchPosition().");}
	public static void punchPositionWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: punchPositionWorld() has been deprecated. Please investigate PunchPosition().");}	
	public static void punchRotation(GameObject target, Hashtable args){Debug.LogError("iTween Error: punchPosition() has been renamed to PunchRotation().");}
	public static void punchRotationWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: punchRotationWorld() has been deprecated. Please investigate PunchRotation().");}	
	public static void punchScale(GameObject target, Hashtable args){Debug.LogError("iTween Error: punchScale() has been renamed to PunchScale().");}
	public static void rotateAdd(GameObject target, Hashtable args){Debug.LogError("iTween Error: rotateAdd() has been renamed to RotateAdd().");}
	public static void rotateBy(GameObject target, Hashtable args){Debug.LogError("iTween Error: rotateBy() has been renamed to RotateBy().");}
	public static void rotateByWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: rotateByWorld() has been deprecated. Please investigate RotateBy().");}
	public static void rotateFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: rotateFrom() has been renamed to RotateFrom().");}
	public static void rotateTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: rotateTo() has been renamed to RotateTo().");}
	public static void scaleAdd(GameObject target, Hashtable args){Debug.LogError("iTween Error: scaleAdd() has been renamed to ScaleAdd().");}
	public static void scaleBy(GameObject target, Hashtable args){Debug.LogError("iTween Error: scaleBy() has been renamed to ScaleBy().");}
	public static void scaleFrom(GameObject target, Hashtable args){Debug.LogError("iTween Error: scaleFrom() has been renamed to ScaleFrom().");}
	public static void scaleTo(GameObject target, Hashtable args){Debug.LogError("iTween Error: scaleTo() has been renamed to ScaleTo().");}
	public static void shake(GameObject target, Hashtable args){Debug.LogError("iTween Error: scale() has been deprecated. Please investigate ShakePosition(), ShakeRotation() and ShakeScale().");}
	public static void shakeWorld(GameObject target, Hashtable args){Debug.LogError("iTween Error: shakeWorld() has been deprecated. Please investigate ShakePosition(), ShakeRotation() and ShakeScale().");}
	public static void stab(GameObject target, Hashtable args){Debug.LogError("iTween Error: stab() has been renamed to Stab().");}
	public static void stop(GameObject target, Hashtable args){Debug.LogError("iTween Error: stop() has been renamed to Stop().");}
	public static void stopType(GameObject target, Hashtable args){Debug.LogError("iTween Error: stopType() has been deprecated. Please investigate Stop().");}
	public static void tweenCount(GameObject target, Hashtable args){Debug.LogError("iTween Error: tweenCount() has been deprecated. Please investigate Count().");}
	*/
    #endregion
}
