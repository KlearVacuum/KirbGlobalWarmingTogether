using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridScript : LayoutGroup
{
    // Rules to prioritize different layout priorities
    public enum eFitType
    {
        UNIFORM,
        WIDTH,
        HEIGHT,
        FIXEDCOLUMNS,
        FIXEDROWS,
        FIXEDCELLSIZE,

        MAX
    }

    public eFitType mFitType;
    public int mRows;
    public int mColumns;
    public Vector2 mCellSize;
    public Vector2 mSpacing;

    public bool autoFitX;
    public bool autoFitY;

    public override void SetLayoutHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        // Cell sizes dont change, adjust by padding and position
        if (mFitType == eFitType.FIXEDCELLSIZE)
        {
            autoFitX = true;
            autoFitY = true;

            // Find total area that buttons must sit within
            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            // Find max number of buttons that can sit with a row/column
            int maxCellsInRow = Mathf.FloorToInt(parentWidth / mCellSize.x);
            int maxCellsInColumn = Mathf.FloorToInt(parentHeight / mCellSize.y);

            // Check if total cells required exceeds max cells allowed in a row
            // if exceed, stack
            mRows = Mathf.CeilToInt((float)transform.childCount / (float)maxCellsInRow);
            mColumns = transform.childCount > maxCellsInRow ? maxCellsInRow : transform.childCount;

            // Set padding to space cells based on parameters above
            mSpacing.x = (parentWidth - ((float)mColumns * mCellSize.x)) / ((float)mColumns);

            if (parentHeight - (mRows * mCellSize.y) > 0)
            {
                mSpacing.y = (parentHeight - ((float)mRows * mCellSize.y)) / ((float)mRows * 2);
            }
            else
            {
                mSpacing.y = 0;
            }

            // Define where cell positions
            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; ++i)
            {
                rowCount = i / mColumns;
                columnCount = i % mColumns;

                RectTransform item = rectChildren[i];

                Vector2 pos = new Vector2(mCellSize.x * columnCount + mSpacing.x * columnCount + padding.left + mSpacing.x / 2,
                                            mCellSize.y * rowCount + mSpacing.y * rowCount + padding.top);

                SetChildAlongAxis(item, 0, pos.x, mCellSize.x);
                SetChildAlongAxis(item, 1, pos.y, mCellSize.y);
            }
        }
        else
        {
            if (mFitType == eFitType.UNIFORM ||
            mFitType == eFitType.WIDTH ||
            mFitType == eFitType.HEIGHT)
            {
                autoFitX = true;
                autoFitY = true;
                float sqrt = Mathf.Sqrt(transform.childCount);
                mRows = Mathf.CeilToInt(sqrt);
                mColumns = Mathf.CeilToInt(sqrt);
            }

            if (mFitType == eFitType.WIDTH || mFitType == eFitType.FIXEDCOLUMNS || mFitType == eFitType.UNIFORM)
            {
                mRows = Mathf.CeilToInt(transform.childCount / (float)mColumns);
            }
            else if (mFitType == eFitType.HEIGHT || mFitType == eFitType.FIXEDROWS || mFitType == eFitType.UNIFORM)
            {
                mColumns = Mathf.CeilToInt(transform.childCount / (float)mRows);
            }

            // Find total area that buttons must sit within
            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            // Define cell dimensions based on parameters calculated above
            float mAutoCellWidth = (parentWidth - (mSpacing.x * (float)(mColumns - 1)) - padding.left - padding.right) / (float)mColumns;
            float mAutoCellHeight = (parentHeight - (mSpacing.y * (float)(mRows - 1)) - padding.top - padding.bottom) / (float)mRows;

            // If autoFit == false, use editor definitions instead
            mCellSize.x = autoFitX ? mAutoCellWidth : mCellSize.x;
            mCellSize.y = autoFitY ? mAutoCellHeight : mCellSize.y;

            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; ++i)
            {
                rowCount = i / mColumns;
                columnCount = i % mColumns;

                RectTransform item = rectChildren[i];

                Vector2 pos = new Vector2(mCellSize.x * columnCount + mSpacing.x * columnCount + padding.left,
                                            mCellSize.y * rowCount + mSpacing.y * rowCount + padding.top);

                SetChildAlongAxis(item, 0, pos.x, mCellSize.x);
                SetChildAlongAxis(item, 1, pos.y, mCellSize.y);
            }
        }
    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
