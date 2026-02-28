using System;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class CanvasGroupExt
    {
        public static void SetActive(this CanvasGroup canvasGroup, bool isActive)
        {
            if (canvasGroup == null)
                throw new ArgumentNullException(nameof(canvasGroup));

            canvasGroup.alpha = isActive ? 1f : 0f;
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }

        public static bool Interactable(this CanvasGroup[] canvasGroups)
        {
            if (canvasGroups == null || canvasGroups.Length == 0)
                return false;

            for (var i = 0; i < canvasGroups.Length; i++)
            {
                if (canvasGroups[i] == null || !canvasGroups[i].interactable)
                    return false;
            }

            return true;
        }
    }
}
