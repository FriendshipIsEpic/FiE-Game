// Amplify Bloom - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
namespace AmplifyBloom
{
	[ExecuteInEditMode]
	[System.Serializable]
	[RequireComponent( typeof( Camera ) )]
	[AddComponentMenu( "Image Effects/Amplify Bloom")]
	public sealed class AmplifyBloomEffect : AmplifyBloomBase { }
}