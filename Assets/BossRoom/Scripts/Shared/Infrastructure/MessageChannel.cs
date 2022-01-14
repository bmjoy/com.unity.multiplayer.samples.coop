using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace BossRoom.Scripts.Shared.Infrastructure
{
    public interface IPublisher<T>
    {
        void Publish(T message);
    }

    public interface ISubscriber<T>
    {
        IDisposable Subscribe(Action<T> handler);
    }

    public static class MessageChannelDIExtenstions
    {
        public static void BindMessageChannel<TMessage>(this DIScope scope)
        {
            scope.BindAsSingle< MessageChannel<TMessage>, IPublisher<TMessage>, ISubscriber<TMessage>>();
        }
    }

    public class MessageChannel<T> : IDisposable, IPublisher<T>, ISubscriber<T>
    {
        readonly Queue<Action> m_PendingHandlers = new Queue<Action>();
        readonly List<Action<T>> m_MessageHandlers = new List<Action<T>>();
        bool m_IsDisposed;

        public void Publish(T message)
        {
            while (m_PendingHandlers.Count > 0)
            {
                m_PendingHandlers.Dequeue()?.Invoke();
            }

            foreach (var messageHandler in m_MessageHandlers)
            {
                messageHandler?.Invoke(message);
            }
        }

        public IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsTrue(!m_MessageHandlers.Contains(handler), $"Attempting to subscribe with the same handler more than once");
            m_PendingHandlers.Enqueue(() => { DoSubscribe(handler); });
            var subscription = new Subscription(this, handler);
            return subscription;

            void DoSubscribe(Action<T> _h)
            {
                if (_h != null && !m_MessageHandlers.Contains(_h))
                    m_MessageHandlers.Add(_h);
            }
        }

        private void Unsubscribe(Action<T> handler)
        {
            m_PendingHandlers.Enqueue(() => { DoUnsubscribe(handler); });

            void DoUnsubscribe(Action<T> _h)
            {
                m_MessageHandlers.Remove(_h);
            }
        }

        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;
                m_MessageHandlers.Clear();
                m_PendingHandlers.Clear();
            }
        }

        private class Subscription : IDisposable
        {
            bool m_isDisposed;
            MessageChannel<T> m_MessageChannel;
            Action<T> m_Handler;

            public Subscription(MessageChannel<T> messageChannel, Action<T> handler)
            {
                m_MessageChannel = messageChannel;
                m_Handler = handler;
            }

            public void Dispose()
            {
                if (!m_isDisposed)
                {
                    m_isDisposed = true;

                    if (!m_MessageChannel.m_IsDisposed)
                    {
                        m_MessageChannel.Unsubscribe(m_Handler);
                    }

                    m_Handler = null;
                    m_MessageChannel = null;
                }
            }
        }
    }
}