using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public struct Matrix<T> where T : struct
{
    private readonly T[] _data;
    private readonly int _rows;
    private readonly int _cols;
    private const int Threshold = 128;

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentException("Rows and columns must be greater than 0.");

        _rows = rows;
        _cols = cols;
        _data = new T[rows * cols];
    }

    public T this[int row, int col]
    {
        get
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                throw new IndexOutOfRangeException("Row or column index is out of range.");
            return _data[row * _cols + col];
        }
        set
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                throw new IndexOutOfRangeException("Row or column index is out of range.");
            _data[row * _cols + col] = value;
        }
    }

    public int Rows => _rows;
    public int Columns => _cols;

    public bool IsEqual(Matrix<T> other)
    {
        if (_rows != other.Rows || _cols != other.Columns)
            return false;

        if (_data.Length < Threshold)
        {
            // For small matrices, use simple comparison.
            for (int i = 0; i < _data.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(_data[i], other._data[i]))
                    return false;
            }
        }
        else
        {
            // For large matrices, use parallel comparison.
            var vectorSize = Vector<T>.Count;
            var results = new ConcurrentBag<bool>();
            var localData = _data;
            Parallel.For(0, _data.Length / vectorSize, (index) =>
            {
                int localIndex = index * vectorSize;
                var v1 = new Vector<T>(localData, localIndex);
                var v2 = new Vector<T>(other._data, localIndex);
                results.Add(v1.Equals(v2));
            });

            if (results.Contains(false))
                return false;

            // Compare remaining elements that are not divisible by vectorSize.
            for (int j = (_data.Length / vectorSize) * vectorSize; j < _data.Length; j++)
            {
                if (!EqualityComparer<T>.Default.Equals(_data[j], other._data[j]))
                    return false;
            }
        }

        return true;
    }

    public Matrix<T> Rotate90Clockwise()
    {
        var rotatedMatrix = new Matrix<T>(_cols, _rows);

        if (_rows > Threshold && _cols > Threshold)
        {
            var localRows = _rows;
            var localCols = _cols;
            var localData = _data;

            // Parallelize the outer loop for large matrices
            Parallel.For(0, _rows, i =>
            {
                for (int j = 0; j < localCols; j++)
                {
                    rotatedMatrix[j, localRows - 1 - i] = localData[i * localCols + j];
                }
            });
        }
        else
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    rotatedMatrix[j, _rows - 1 - i] = _data[i * _cols + j];
                }
            }
        }

        return rotatedMatrix;
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _cols; j++)
            {
                result.Append(this[i, j]);
            }
        }
        return result.ToString();
    }

    public T[] GetData()
    {
        return _data;
    }
}
