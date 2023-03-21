using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Arduino.Core.Messages
{
    internal class ApplicationInitializedMessage : ValueChangedMessage<bool>
    {
        public ApplicationInitializedMessage(bool value) : base(value)
        {
        }
    }
}
