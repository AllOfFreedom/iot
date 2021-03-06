﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Gpio;
using System.Threading.Tasks;
using CommandLine;
using DeviceApiTester.Infrastructure;

namespace DeviceApiTester.Commands.Gpio
{
    [Verb("gpio-blink-led", HelpText = "Blinks an LED connected to a specified GPIO pin.")]
    public class BlinkLed : GpioCommand, ICommandVerbAsync
    {
        /// <summary>Executes the command asynchronously.</summary>
        /// <returns>The command's exit code.</returns>
        /// <remarks>
        ///     NOTE: This test app uses the base class's <see cref="CreateGpioController"/> method to create a device.<br/>
        ///     Real-world usage would simply create an instance of <see cref="GpioController"/>:
        ///     <code>using (var gpio = new GpioController())</code>
        /// </remarks>
        public async Task<int> ExecuteAsync()
        {
            Console.WriteLine($"LedPin={LedPin}, Scheme={Scheme}, Count={Count}, TimeOn={TimeOn} ms, TimeOff={TimeOff} ms, Driver={Driver}");

            using (var gpio = CreateGpioController())
            {
                gpio.OpenPin(LedPin, PinMode.Output);
                gpio.Write(LedPin, OffValue);

                for (int index = 0; index < Count; ++index)
                {
                    Console.WriteLine($"[{index}] Turn the LED on and wait {TimeOn} ms");
                    gpio.Write(LedPin, OnValue);
                    await Task.Delay(TimeOn);

                    Console.WriteLine($"[{index}] Turn the LED off and wait {TimeOff} ms");
                    gpio.Write(LedPin, OffValue);
                    await Task.Delay(TimeOff);
                }
            }

            return 0;
        }

        [Option('l', "led-pin", HelpText = "The GPIO pin which the LED is connected to, numbered based on the --scheme argument", Required = true)]
        public int LedPin { get; set; }

        [Option('c', "count", HelpText = "The number of times to blink the LED", Required = false, Default = 5)]
        public int Count { get; set; }

        [Option("time-on", HelpText = "The number of milliseconds to keep the LED on for each blink", Required = false, Default = 200)]
        public int TimeOn { get; set; }

        [Option("time-off", HelpText = "The number of milliseconds to keep the LED off for each blink", Required = false, Default = 200)]
        public int TimeOff { get; set; }

        [Option("on-value", HelpText = "The value that turns the LED on: { High | Low }", Required = false, Default = PinValue.High)]
        public PinValue OnValue { get; set; }

        private PinValue OffValue
        {
            get { return OnValue == PinValue.High ? PinValue.Low : PinValue.High; }
        }
    }
}
