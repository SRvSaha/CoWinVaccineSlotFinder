#!/bin/bash

DISTRIBUTION=$(lsb_release -ar 2>/dev/null | grep "Distributor ID:" | cut -f 2)

if [[ ! $(dpkg -l | grep pulseaudio-utils) ]]; then
  case $DISTRIBUTION in
    Ubuntu | Debian | Kali | Raspbian)
      sudo apt-get update && sudo apt-get install pulseaudio-utils libc6-dev libgdiplus
      ;;
    Arch)
      sudo pacman -S libpulse
      ;;
    CentOS)
      sudo yum install pulseaudio-utils
      ;;
    Fedora)
      sudo dnf install pulseaudio-utils
      ;;
    *)
      echo "Please install the package \"pulseaudio-utils\" before using this application"
      exit 0
      ;;
  esac
fi

if [[ ! $(dpkg -l | grep libgdiplus) || ! $(dpkg -l | grep libc6-dev) ]]; then
  case $DISTRIBUTION in
    Ubuntu | Debian | Kali | Raspbian)
      sudo apt-get update && sudo apt-get install libc6-dev libgdiplus
      ;;
    *)
      echo "========================================================================================================================================================================"
      echo "We see that you are using a non Ubuntu, Debian, Kali or Raspbian Linux distribution."
      echo "While running the application if you get the Svg.SvgGdiPlusCannotBeLoadedException, then please try installing the packages \"libgdiplus\" and \"libc6-dev\""
      echo "========================================================================================================================================================================"
      echo ""
      exit 0
      ;;
  esac
fi
chmod +x CoWinVaccineSlotFinder
./CoWinVaccineSlotFinder
