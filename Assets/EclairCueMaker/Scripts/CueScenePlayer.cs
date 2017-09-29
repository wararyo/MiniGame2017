﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace wararyo.EclairCueMaker
{
    public class CueScenePlayer : MonoBehaviour
    {
		public CueScene cueScene;

		public bool playOnAwake = true;
		public bool loop = false;

        private float time = 0;

        private int cursor = 0;

		private bool isPlaying = false;

        public int Cursor
        {
            get
            {
                return cursor;
            }
            set
            {
                OnCursorChanged(cursor, value);
                cursor = value;
            }
        }

        protected virtual void OnCursorChanged(int before, int after) {; }

		/// <summary>
		/// Called before invoke next cue.
		/// If return false, invoking will be canceled.
		/// </summary>
		protected virtual bool OnInvoking(){
			return true;
		}

        protected virtual void Start()
        {
			if (playOnAwake)
				Play ();
        }

        protected virtual void Update()
        {
			if (isPlaying) {
				time += Time.deltaTime;
				if (cueScene.Count == cursor) {
					if (loop) {
						cursor = 0;
						time = 0;
					}
					else
						Stop ();
				}
				if (0 <= cueScene.cueList [cursor].time) {
					if (cueScene.cueList [cursor].time < time) {
						Invoke ();
					}
				}
			}
        }

        public void Invoke()
        {
			if (OnInvoking ()) {
				if (cueScene.cueList [cursor].gameObjectName != "") {
					Cue.Invoke (cueScene.cueList [cursor]);
				}
				time = 0;
				if (cueScene.Count - 1 > Cursor)
					Cursor++;
				else if (loop)
					Cursor = 0;
			}
        }

		public void Play(){
			isPlaying = true;
		}

		public void Play(CueScene cuescene){
			this.cueScene = cuescene;
			cursor = 0;
			time = 0;
			Play ();
		}

		public void Stop(){
			Pause ();
			cursor = 0;
			time = 0;
		}

		public void Pause(){
			isPlaying = false;
		}
    }

}