using System;
using System.Collections.Generic;

namespace Fie.Object
{
	public interface FieStateMachineInterface
	{
		event StateChangeDelegate stateChangeEvent;

		void initialize(FieGameCharacter gameCharacter);

		void terminate(FieGameCharacter gameCharacter);

		void terminateAndCallStateChangeEvent(FieGameCharacter gameCharacter, Type fromState, Type toState);

		void updateState<T>(ref T gameCharacter) where T : FieGameCharacter;

		bool isFinished();

		bool isEnd();

		Type getNextState();

		List<Type> getAllowedStateList();

		float getDelay();

		bool isNotNetworkSync();
	}
}
