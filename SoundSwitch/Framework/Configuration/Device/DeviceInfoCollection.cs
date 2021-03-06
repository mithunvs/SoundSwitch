﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAudio.CoreAudioApi;

namespace SoundSwitch.Framework.Configuration.Device
{
    public class DeviceInfoCollection : ICollection<DeviceInfo>
    {
        private readonly Dictionary<string, DeviceInfo> _deviceByName = new Dictionary<string, DeviceInfo>();
        private readonly Dictionary<string, DeviceInfo> _deviceById = new Dictionary<string, DeviceInfo>();

        private readonly HashSet<DeviceInfo> _original;

        public DeviceInfoCollection(HashSet<DeviceInfo> devices)
        {
            _original = devices;
            foreach (var deviceInfo in devices)
            {
                AddItem(deviceInfo);
            }
        }

        /// <summary>
        /// Intersect with MMDevices
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public IEnumerable<MMDevice> IntersectWith(IEnumerable<MMDevice> devices)
        {
            var devicesResult = new Dictionary<string, MMDevice>();
            foreach (var mmDevice in devices)
            {
                if (devicesResult.ContainsKey(mmDevice.ID))
                    continue;

                if (!_deviceById.ContainsKey(mmDevice.ID) && !_deviceByName.ContainsKey(mmDevice.FriendlyName))
                    continue;

                devicesResult.Add(mmDevice.ID, mmDevice);

            }

            return devicesResult.Values;

        }

        public IEnumerator<DeviceInfo> GetEnumerator()
        {
            return _deviceById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(DeviceInfo item)
        {
            AddItem(item, true);
        }

        /// <summary>
        /// Add Item and update or not the original
        /// </summary>
        /// <param name="item"></param>
        /// <param name="updateOriginal"></param>
        private void AddItem(DeviceInfo item, bool updateOriginal = false)
        {
            if (item == null)
            {
                return;
            }

            try
            {
                _deviceById.Add(item.Id, item);
            }
            catch (ArgumentException)
            {
            }

            try
            {
                _deviceByName.Add(item.Name, item);
            }
            catch (ArgumentException)
            {
            }

            if (updateOriginal)
            {
                _original.Add(item);
            }
        }

        public void Clear()
        {
            _deviceById.Clear();
            _deviceByName.Clear();
            _original.Clear();
        }

        public bool Contains(DeviceInfo item)
        {
            return item != null && (_deviceById.ContainsKey(item.Id) || _deviceByName.ContainsKey(item.Name));
        }

        public void CopyTo(DeviceInfo[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(DeviceInfo item)
        {
            if (item == null)
            {
                return false;
            }

            var result = _original.RemoveWhere((info => info?.Id == item.Id || info?.Name == item.Name)) > 0;

            try
            {
                result &= _deviceById.Remove(item.Id);
            }
            catch (ArgumentException)
            {

            }

            try
            {
                result &= _deviceByName.Remove(item.Name);
            }
            catch (ArgumentException)
            {

            }

            return result;
        }

        public int Count => _deviceById.Count;

        public bool IsReadOnly => false;
    }
}