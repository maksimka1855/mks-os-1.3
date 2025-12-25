# MKS-OS 1.3

**A simple operating system built with COSMOS User Kit.**

## About

MKS-OS is a lightweight educational operating system prototype developed using the COSMOS (C# Open Source Managed Operating System) toolkit. It demonstrates basic OS functionality such as command processing, system information reporting, and hardware interaction.

## Requirements

- Visual Studio 2022 (Community/Professional/Enterprise)
- COSMOS User Kit (latest version)
- .NET SDK (included with VS 2022)
- Administrator privileges (for booting via ISO/VM)

## How to Build and Run

1. Install Visual Studio 2022 and COSMOS User Kit.
2. Open Visual Studio and create a new project:
   - Select **COSMOS Kernel** template.
3. Replace the default `Kernel.cs` content:
   - Delete the existing code in `Kernel.cs`.
   - Paste the code from the `source/kernel.cs` file into your project's `Kernel.cs`.
4. Build the project (Ctrl+Shift+B).
5. Run the OS:
   - Press F5 to launch in a virtual machine (QEMU/VMware).
   - Or generate an ISO (`Build → Create ISO`) and boot it in a VM/physical machine.

## Commands

| Command    | Description |
|------------|-------------|
| `help`     | Display help for all commands. |
| `info`     | Show system information (RAM, CPU, uptime). |
| `beep`     | Produce a beep sound (800 Hz, 200 ms). |
| `clear`    | Clear the console screen. |
| `version`  | Display the OS version (MKS-OS 1.3). |
| `shutdown` | Shut down the system. |

### Examples

- View system info:  
info
RAM: 512 MB | CPU: QEMU vCPU | Uptime: 00:02:15

- Clear the screen:  
clear


## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.


                       [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
