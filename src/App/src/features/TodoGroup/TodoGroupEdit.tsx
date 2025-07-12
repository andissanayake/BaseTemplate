import React from "react";
import {
  Form,
  Input,
  notification,
  Select,
  Button,
  Space,
  Typography,
} from "antd";
import { useTodoGroupStore } from "./todoGroupStore";
import { useNavigate, useParams } from "react-router-dom";
import { TodoGroupService } from "./todoGroupService";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleResult } from "../../common/handleResult";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { handleServerError } from "../../common/serverErrorHandler";

const TodoGroupEdit: React.FC = () => {
  const { setLoading } = useTodoGroupStore();

  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { listId } = useParams();
  if (!listId) throw new Error("listId is required");

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      values.id = +listId;
      setLoading(true);
      const response = await TodoGroupService.updateTodoGroup(values);
      return handleResult(response, {
        onSuccess: () => {
          notification.success({
            message: "Todo list updated successfully!",
          });
          navigate("/todo-list");
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: (errors) => {
          handleServerError(errors, "Failed to update todo list!");
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  useAsyncEffect(async () => {
    form.resetFields();
    setLoading(true);
    const response = await TodoGroupService.fetchTodoGroupById(listId);
    handleResult(response, {
      onSuccess: (data) => {
        form.setFieldsValue(data);
      },
      onServerError: (errors) => {
        handleServerError(errors, "Failed to fetch todo list item!");
      },

      onFinally: () => {
        setLoading(false);
      },
    });
  }, [listId, form]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Edit
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo list Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo list name!" },
          ]}
        >
          <Input placeholder="Enter todo list name" />
        </Form.Item>
        <Form.Item
          label="Select Colour"
          name="colour"
          rules={[{ required: true, message: "Please select a colour!" }]}
        >
          <Select optionLabelProp="label">
            {TodoGroupService.getColours().map((colour) => (
              <Select.Option
                key={colour.value}
                value={colour.value}
                label={
                  <span style={{ display: "flex", alignItems: "center" }}>
                    <span
                      style={{
                        display: "inline-block",
                        width: 20,
                        height: 20,
                        backgroundColor: colour.value,
                        marginRight: 10,
                        borderRadius: "50%",
                      }}
                    />
                    {colour.label}
                  </span>
                }
              >
                <span style={{ display: "flex", alignItems: "center" }}>
                  <span
                    style={{
                      display: "inline-block",
                      width: 20,
                      height: 20,
                      backgroundColor: colour.value,
                      marginRight: 10,
                      borderRadius: "50%",
                    }}
                  />
                  {colour.label}
                </span>
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => navigate("/todo-list")}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </>
  );
};

export default TodoGroupEdit;
