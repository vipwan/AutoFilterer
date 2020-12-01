﻿using AutoFilterer.Tests.Environment.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFilterer.Extensions;
using AutoFilterer.Enums;

namespace AutoFilterer.Tests.Extensions
{
    public class ExpressionExtensionsTests
    {
        Expression leftExpression, rightExpression;

        public ExpressionExtensionsTests()
        {
            var parameter = Expression.Parameter(typeof(BookFilterBase), "x");

            leftExpression = Expression.Equal(
                                    Expression.Property(parameter, nameof(BookFilterBase.Author)),
                                    Expression.Constant("a"));

            rightExpression = Expression.Equal(
                                    Expression.Property(parameter, nameof(BookFilterBase.Title)),
                                    Expression.Constant("b"));
        }

        [Fact]
        public void Combine_WithTwoExpressionAnd_ShouldCombineWithAndAlso()
        {
            // Arrange
            var combineType = CombineType.And;

            var expected = Expression.AndAlso(leftExpression, rightExpression);

            // Act
            var actual = leftExpression.Combine(rightExpression, combineType);

            // Assert
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void Combine_WithTwoExpressionOr_ShouldCombineWithOrElse()
        {
            // Arrange
            var combineType = CombineType.Or;
            var expected = Expression.OrElse(leftExpression, rightExpression);

            // Act
            var actual = leftExpression.Combine(rightExpression, combineType);

            // Assert
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void Combine_WithNullLeftExpressionOr_ShouldReturnRight()
        {
            // Arrange
            var combineType = CombineType.Or;
            Expression leftNullExpression = null;
            var expected = rightExpression;

            // Act
            var actual = leftNullExpression.Combine(rightExpression, combineType);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Combine_WithNullRightExpressionAnd_ShouldReturnRight()
        {
            // Arrange
            var combineType = CombineType.And;
            Expression rightNullExpression = null;
            var expected = leftExpression;

            // Act
            var actual = leftExpression.Combine(rightNullExpression, combineType);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
